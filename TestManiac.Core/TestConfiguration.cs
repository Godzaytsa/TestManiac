namespace TestManiac.Core;

/// <summary>
/// Configuration for the web testing automation
/// </summary>
public class TestConfiguration
{
    /// <summary>
    /// The URL of the website to test
    /// </summary>
    public string BaseUrl { get; set; } = string.Empty;

    /// <summary>
    /// The URL of the page to start testing from (optional)
    /// If not specified, testing starts from BaseUrl
    /// </summary>
    public string? StartUrl { get; set; }

    /// <summary>
    /// Username for login (optional)
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password for login (optional)
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// CSS selector for the username input field
    /// </summary>
    public string? UsernameSelector { get; set; } =
        "xpath=//input[contains(@name, 'UserName') or contains(@name, 'UserName') or contains(@name, 'username')]";

    /// <summary>
    /// CSS selector for the password input field
    /// </summary>
    public string? PasswordSelector { get; set; } = "xpath=//input[@type='password']";

    /// <summary>
    /// CSS selector for the login submit button
    /// </summary>
    public string? LoginButtonSelector { get; set; } = "xpath=//button[@type='submit']";

    /// <summary>
    /// Maximum number of pages to crawl (default: 50)
    /// </summary>
    public int MaxPagesToCrawl { get; set; } = 50;

    /// <summary>
    /// Maximum depth of page navigation (default: 5)
    /// </summary>
    public int MaxDepth { get; set; } = 5;

    /// <summary>
    /// Timeout for page navigation in milliseconds (default: 30000)
    /// </summary>
    public int NavigationTimeout { get; set; } = 30000;

    /// <summary>
    /// Whether to run browser in headless mode (default: true)
    /// </summary>
    public bool Headless { get; set; } = false;

    /// <summary>
    /// Browser type to use (Chromium, Firefox, or WebKit)
    /// </summary>
    public BrowserType BrowserType { get; set; } = BrowserType.Chromium;

    /// <summary>
    /// Delay between interactions in milliseconds (default: 500)
    /// </summary>
    public int InteractionDelay { get; set; } = 500;

    /// <summary>
    /// Timeout for click interactions in milliseconds (default: 5000)
    /// </summary>
    public int ClickTimeout { get; set; } = 10000;

    /// <summary>
    /// Whether to wait for network idle after clicks (default: true)
    /// Waits until there are no more than 2 network connections for at least 500ms
    /// </summary>
    public bool WaitForNetworkIdle { get; set; } = true;

    /// <summary>
    /// Timeout for waiting for network idle in milliseconds (default: 10000)
    /// </summary>
    public int NetworkIdleTimeout { get; set; } = 10000;

    /// <summary>
    /// Whether to take screenshots on errors (default: true)
    /// </summary>
    public bool ScreenshotOnError { get; set; } = true;

    /// <summary>
    /// Path to save screenshots (default: ./screenshots)
    /// </summary>
    public string ScreenshotPath { get; set; } = "./screenshots";

    /// <summary>
    /// List of URL patterns to exclude from testing (supports wildcards)
    /// </summary>
    public List<string> ExcludeUrls { get; set; } = new();

    /// <summary>
    /// Whether to exclude the login page from testing (default: true)
    /// </summary>
    public bool ExcludeLoginPage { get; set; } = true;
}

/// <summary>
/// Browser type enumeration
/// </summary>
public enum BrowserType
{
    Chromium,
    Firefox,
    WebKit
}
