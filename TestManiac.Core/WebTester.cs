using Microsoft.Playwright;

namespace TestManiac.Core;

/// <summary>
/// Main class for automated web testing
/// </summary>
public class WebTester : IAsyncDisposable
{
    private readonly TestConfiguration _config;
    private IPlaywright? _playwright;
    private IBrowser? _browser;
    private IBrowserContext? _context;
    private IPage? _page;
    private readonly HashSet<string> _visitedUrls = new();
    private readonly TestSummary _summary = new();
    private readonly Uri _baseUri;
    private int _screenshotCounter = 0;

    public event EventHandler<string>? LogMessage;
    public event EventHandler<TestResult>? InteractionCompleted;
    public event EventHandler<string>? PageVisited;

    public WebTester(TestConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _baseUri = new Uri(_config.BaseUrl);
        _summary.StartTime = DateTime.Now;

        // Create screenshot directory if needed
        if (_config.ScreenshotOnError && !Directory.Exists(_config.ScreenshotPath))
        {
            Directory.CreateDirectory(_config.ScreenshotPath);
        }
    }

    /// <summary>
    /// Initialize Playwright and browser
    /// </summary>
    public async Task InitializeAsync()
    {
        Log("Initializing Playwright...");
        _playwright = await Playwright.CreateAsync();

        Log($"Launching {_config.BrowserType} browser...");
        _browser = _config.BrowserType switch
        {
            BrowserType.Firefox => await _playwright.Firefox.LaunchAsync(new() { Headless = _config.Headless }),
            BrowserType.WebKit => await _playwright.Webkit.LaunchAsync(new() { Headless = _config.Headless }),
            _ => await _playwright.Chromium.LaunchAsync(new() { Headless = _config.Headless })
        };

        _context = await _browser.NewContextAsync(new()
        {
            ViewportSize = new() { Width = 1920, Height = 1080 },
            IgnoreHTTPSErrors = true
        });

        _page = await _context.NewPageAsync();
        _page.SetDefaultTimeout(_config.NavigationTimeout);

        // Set up console message and error listeners
        _page.Console += (_, msg) =>
        {
            if (msg.Type == "error")
            {
                Log($"Console Error: {msg.Text}");
            }
        };

        _page.PageError += (_, error) =>
        {
            Log($"Page Error: {error}");
        };

        Log("Browser initialized successfully.");
    }

    /// <summary>
    /// Run the complete test workflow
    /// </summary>
    public async Task<TestSummary> RunTestsAsync()
    {
        try
        {
            if (_page == null)
            {
                await InitializeAsync();
            }

            Log($"Navigating to base URL: {_config.BaseUrl}");
            await _page!.GotoAsync(_config.BaseUrl, new() { WaitUntil = WaitUntilState.NetworkIdle });

            // Perform login if credentials provided
            if (!string.IsNullOrEmpty(_config.Username) &&
                !string.IsNullOrEmpty(_config.Password)/* &&
                !string.IsNullOrEmpty(_config.UsernameSelector) &&
                !string.IsNullOrEmpty(_config.PasswordSelector) &&
                !string.IsNullOrEmpty(_config.LoginButtonSelector)*/)
            {
                await PerformLoginAsync();
            }
            else
            {
                Log("No login credentials provided, skipping login step.");
            }

            // Start crawling from the base page
            await CrawlPageAsync(_page.Url, 0);

            _summary.EndTime = DateTime.Now;
            Log($"\n=== Test Summary ===");
            Log($"Duration: {_summary.Duration}");
            Log($"Pages Visited: {_summary.TotalPagesVisited}");
            Log($"Total Interactions: {_summary.TotalInteractions}");
            Log($"Successful: {_summary.SuccessfulInteractions}");
            Log($"Failed: {_summary.FailedInteractions}");
            Log($"Success Rate: {_summary.SuccessRate:F2}%");

            return _summary;
        }
        catch (Exception ex)
        {
            Log($"Fatal error during test execution: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Perform login with provided credentials
    /// </summary>
    private async Task PerformLoginAsync()
    {
        try
        {
            Log("Attempting to login...");

            await _page!.FillAsync(_config.UsernameSelector!, _config.Username!);
            await _page.FillAsync(_config.PasswordSelector!, _config.Password!);
            await _page.ClickAsync(_config.LoginButtonSelector!);

            // Wait for navigation or network idle after login
            try
            {
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle, new() { Timeout = 10000 });
            }
            catch
            {
                // Network idle might timeout, continue anyway
            }

            Log("Login completed.");
        }
        catch (Exception ex)
        {
            Log($"Login failed: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Crawl a page and interact with all elements
    /// </summary>
    private async Task CrawlPageAsync(string url, int depth)
    {
        // Check if we should continue crawling
        if (depth > _config.MaxDepth ||
            _visitedUrls.Count >= _config.MaxPagesToCrawl ||
            _visitedUrls.Contains(url))
        {
            return;
        }

        // Check if URL belongs to the same domain
        if (!IsSameDomain(url))
        {
            Log($"Skipping URL (different domain): {url}");
            return;
        }

        // Check if URL should be excluded
        if (ShouldExcludeUrl(url))
        {
            Log($"Skipping URL (excluded): {url}");
            _visitedUrls.Add(url); // Mark as visited to avoid checking again
            return;
        }

        _visitedUrls.Add(url);
        _summary.TotalPagesVisited++;
        _summary.VisitedUrls.Add(url);
        PageVisited?.Invoke(this, url);

        Log($"\n[Depth {depth}] Visiting: {url}");

        try
        {
            // Navigate to the page if not already there
            if (_page!.Url != url)
            {
                await _page.GotoAsync(url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
                await Task.Delay(_config.InteractionDelay);
            }

            // Get all interactable elements
            var interactableElements = await GetInteractableElementsAsync();
            Log($"Found {interactableElements.Count} interactable elements on this page.");

            // Track links for further crawling
            var linksToVisit = new List<string>();

            // Interact with each element
            for (int i = 0; i < interactableElements.Count; i++)
            {
                try
                {
                    // Re-fetch elements each time to avoid stale element references
                    var currentElements = await GetInteractableElementsAsync();

                    // If we've processed more elements than currently exist, we're done
                    if (i >= currentElements.Count)
                    {
                        Log($"    Element index {i} no longer exists, stopping interactions on this page.");
                        break;
                    }

                    var element = currentElements[i];

                    // Get element info before interaction (wrapped in try-catch for stale elements)
                    string tagName, elementText, description;
                    string? href = null;

                    try
                    {
                        tagName = await element.EvaluateAsync<string>("el => el.tagName");
                        elementText = await element.TextContentAsync() ?? "";
                        href = await element.GetAttributeAsync("href");
                        description = $"{tagName} - '{elementText.Trim().Substring(0, Math.Min(50, elementText.Trim().Length))}'";
                    }
                    catch (Exception)
                    {
                        // Element became stale while getting info, skip it
                        Log($"  [{i + 1}/{interactableElements.Count}] Element became stale, skipping...");
                        continue;
                    }

                    Log($"  [{i + 1}/{interactableElements.Count}] Interacting with: {description}");

                    // Check if it's a link
                    bool isLink = tagName.ToLower() == "a" && !string.IsNullOrEmpty(href);

                    if (isLink)
                    {
                        var absoluteUrl = new Uri(_baseUri, href).ToString();
                        if (IsSameDomain(absoluteUrl) && !_visitedUrls.Contains(absoluteUrl))
                        {
                            linksToVisit.Add(absoluteUrl);
                        }
                    }

                    // Perform the click
                    var currentUrl = _page.Url;

                    try
                    {
                        await element.ClickAsync(new() { Timeout = 5000 });
                        await Task.Delay(_config.InteractionDelay);
                    }
                    catch (Exception clickEx)
                    {
                        // Element might have become detached, log and continue
                        if (clickEx.Message.Contains("Execution context was destroyed") ||
                            clickEx.Message.Contains("detached"))
                        {
                            Log($"    ⚠ Element detached during click, skipping...");
                            continue;
                        }
                        throw; // Re-throw other exceptions
                    }

                    // Check for errors after interaction
                    var hasError = await CheckForErrorsAsync();

                    var result = new TestResult
                    {
                        Url = url,
                        ElementDescription = description,
                        Success = !hasError,
                        ErrorMessage = hasError ? "Error detected on page after interaction" : null,
                        Timestamp = DateTime.Now
                    };

                    if (hasError)
                    {
                        result.ScreenshotPath = await TakeScreenshotAsync($"error_{_screenshotCounter++}");
                        _summary.FailedInteractions++;
                        Log($"    ❌ Error detected!");
                    }
                    else
                    {
                        _summary.SuccessfulInteractions++;
                        Log($"    ✓ Success");
                    }

                    _summary.TotalInteractions++;
                    _summary.Results.Add(result);
                    InteractionCompleted?.Invoke(this, result);

                    // If page changed (navigation occurred), go back and refetch elements
                    if (_page.Url != currentUrl)
                    {
                        Log($"    Page changed, navigating back...");
                        await _page.GoBackAsync(new() { WaitUntil = WaitUntilState.DOMContentLoaded });
                        await Task.Delay(_config.InteractionDelay);

                        // Elements are now stale, they'll be refetched in next iteration
                    }
                }
                catch (Exception ex)
                {
                    Log($"    ⚠ Exception during interaction: {ex.Message}");

                    var result = new TestResult
                    {
                        Url = url,
                        ElementDescription = "Unknown element",
                        Success = false,
                        ErrorMessage = ex.Message,
                        ErrorType = ex.GetType().Name,
                        Timestamp = DateTime.Now,
                        ScreenshotPath = await TakeScreenshotAsync($"exception_{_screenshotCounter++}")
                    };

                    _summary.TotalInteractions++;
                    _summary.FailedInteractions++;
                    _summary.Results.Add(result);
                    InteractionCompleted?.Invoke(this, result);

                    // Try to recover
                    try
                    {
                        await _page.GotoAsync(url, new() { WaitUntil = WaitUntilState.DOMContentLoaded });
                        await Task.Delay(_config.InteractionDelay);
                    }
                    catch
                    {
                        Log($"    Failed to recover, skipping remaining elements on this page.");
                        break;
                    }
                }
            }

            // Recursively crawl discovered links
            foreach (var link in linksToVisit)
            {
                await CrawlPageAsync(link, depth + 1);
            }
        }
        catch (Exception ex)
        {
            Log($"Error crawling page {url}: {ex.Message}");
        }
    }

    /// <summary>
    /// Get all visible and enabled interactable elements
    /// </summary>
    private async Task<List<IElementHandle>> GetInteractableElementsAsync()
    {
        var elements = new List<IElementHandle>();

        try
        {
            // Query for common interactable elements
            var selectors = new[]
            {
                "button:visible:not([disabled])",
                "a:visible:not([disabled])",
                "input[type='button']:visible:not([disabled])",
                "input[type='submit']:visible:not([disabled])",
                "[role='button']:visible:not([disabled])",
                "[onclick]:visible:not([disabled])"
            };

            foreach (var selector in selectors)
            {
                try
                {
                    var found = await _page!.QuerySelectorAllAsync(selector);

                    // Filter to only visible and enabled elements
                    foreach (var elem in found)
                    {
                        try
                        {
                            var isVisible = await elem.IsVisibleAsync();
                            var isEnabled = await elem.IsEnabledAsync();

                            if (isVisible && isEnabled)
                            {
                                elements.Add(elem);
                            }
                        }
                        catch
                        {
                            // Element might have been removed from DOM
                        }
                    }
                }
                catch
                {
                    // Continue with next selector
                }
            }
        }
        catch (Exception ex)
        {
            Log($"Error getting interactable elements: {ex.Message}");
        }

        return elements;
    }

    /// <summary>
    /// Check if errors occurred on the page
    /// </summary>
    private async Task<bool> CheckForErrorsAsync()
    {
        try
        {
            // Check for common error indicators
            var errorSelectors = new[]
            {
                "[class*='error']:visible",
                "[class*='alert-danger']:visible",
                "[role='alert']:visible",
                ".error-message:visible",
                "#error:visible"
            };

            foreach (var selector in errorSelectors)
            {
                try
                {
                    var errorElement = await _page!.QuerySelectorAsync(selector);
                    if (errorElement != null)
                    {
                        var isVisible = await errorElement.IsVisibleAsync();
                        if (isVisible)
                        {
                            var text = await errorElement.TextContentAsync();
                            if (!string.IsNullOrWhiteSpace(text))
                            {
                                Log($"    Error element found: {text.Trim().Substring(0, Math.Min(100, text.Trim().Length))}");
                                return true;
                            }
                        }
                    }
                }
                catch
                {
                    // Continue checking other selectors
                }
            }

            // Note: More sophisticated error checking could be added here
            // such as checking HTTP response status codes

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Take a screenshot
    /// </summary>
    private async Task<string?> TakeScreenshotAsync(string fileName)
    {
        if (!_config.ScreenshotOnError || _page == null)
        {
            return null;
        }

        try
        {
            var path = Path.Combine(_config.ScreenshotPath, $"{fileName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            await _page.ScreenshotAsync(new() { Path = path, FullPage = true });
            return path;
        }
        catch (Exception ex)
        {
            Log($"Failed to take screenshot: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// Check if URL belongs to the same domain
    /// </summary>
    private bool IsSameDomain(string url)
    {
        try
        {
            var uri = new Uri(url);
            return uri.Host.Equals(_baseUri.Host, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Check if URL should be excluded from testing
    /// </summary>
    private bool ShouldExcludeUrl(string url)
    {
        try
        {
            // If ExcludeLoginPage is true and this is the login page, exclude it
            if (_config.ExcludeLoginPage && url.Equals(_config.BaseUrl, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Check against exclude patterns
            if (_config.ExcludeUrls != null && _config.ExcludeUrls.Any())
            {
                var uri = new Uri(url);
                var urlPath = uri.AbsolutePath.ToLower();
                var fullUrl = url.ToLower();

                foreach (var pattern in _config.ExcludeUrls)
                {
                    var patternLower = pattern.ToLower();

                    // Exact match
                    if (fullUrl == patternLower || urlPath == patternLower)
                    {
                        return true;
                    }

                    // Wildcard matching (simple contains check)
                    if (patternLower.Contains("*"))
                    {
                        var regexPattern = "^" + System.Text.RegularExpressions.Regex.Escape(patternLower)
                            .Replace("\\*", ".*") + "$";
                        if (System.Text.RegularExpressions.Regex.IsMatch(fullUrl, regexPattern) ||
                            System.Text.RegularExpressions.Regex.IsMatch(urlPath, regexPattern))
                        {
                            return true;
                        }
                    }

                    // Contains match
                    if (fullUrl.Contains(patternLower) || urlPath.Contains(patternLower))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Log a message
    /// </summary>
    private void Log(string message)
    {
        LogMessage?.Invoke(this, message);
    }

    /// <summary>
    /// Dispose resources
    /// </summary>
    public async ValueTask DisposeAsync()
    {
        if (_page != null)
        {
            await _page.CloseAsync();
        }

        if (_context != null)
        {
            await _context.CloseAsync();
        }

        if (_browser != null)
        {
            await _browser.CloseAsync();
        }

        _playwright?.Dispose();
    }
}
