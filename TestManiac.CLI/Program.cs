using System.Text.Json;
using System.Text.Json.Serialization;
using TestManiac.Core;

namespace TestManiac.CLI;

class Program
{
    static async Task<int> Main(string[] args)
    {
        Console.WriteLine("╔═══════════════════════════════════════╗");
        Console.WriteLine("║           TestManiac v1.0             ║");
        Console.WriteLine("║      Automated Web Testing Tool       ║");
        Console.WriteLine("║                                       ║");
        Console.WriteLine("║          Created by Copilot           ║");
        Console.WriteLine("║ in collaboration with Ievgen Solodkyi ║");
        Console.WriteLine("║                                       ║");
        Console.WriteLine("║    Inspired by Oleksandra Solodka     ║");
        Console.WriteLine("╚═══════════════════════════════════════╝\n");

        try
        {
            // Parse command line arguments
            var config = ParseArguments(args);

            if (config == null)
            {
                ShowUsage();
                return 1;
            }

            // Display configuration
            Console.WriteLine("Configuration:");
            Console.WriteLine($"  Base URL: {config.BaseUrl}");
            Console.WriteLine($"  Browser: {config.BrowserType}");
            Console.WriteLine($"  Headless: {config.Headless}");
            Console.WriteLine($"  Max Pages: {config.MaxPagesToCrawl}");
            Console.WriteLine($"  Max Depth: {config.MaxDepth}");
            Console.WriteLine($"  Login: {(!string.IsNullOrEmpty(config.Username) ? "Yes" : "No")}");
            Console.WriteLine();

            // Run tests
            await using var tester = new WebTester(config);

            // Subscribe to events
            tester.LogMessage += (sender, message) => Console.WriteLine(message);
            tester.PageVisited += (sender, url) => Console.WriteLine($"📄 Visiting: {url}");
            tester.InteractionCompleted += (sender, result) =>
            {
                if (!result.Success)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"   ❌ Failed: {result.ElementDescription}");
                    if (!string.IsNullOrEmpty(result.ErrorMessage))
                    {
                        Console.WriteLine($"      Error: {result.ErrorMessage}");
                    }
                    Console.ResetColor();
                }
            };

            Console.WriteLine("Starting tests...\n");
            var summary = await tester.RunTestsAsync();

            // Save results to JSON file
            var resultsPath = $"test-results_{DateTime.Now:yyyyMMdd_HHmmss}.json";
            await SaveResultsAsync(summary, resultsPath);
            Console.WriteLine($"\n✓ Results saved to: {resultsPath}");

            // Display summary
            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("TEST SUMMARY");
            Console.WriteLine(new string('=', 50));
            Console.WriteLine($"Duration:              {summary.Duration}");
            Console.WriteLine($"Pages Visited:         {summary.TotalPagesVisited}");
            Console.WriteLine($"Total Interactions:    {summary.TotalInteractions}");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Successful:            {summary.SuccessfulInteractions}");
            Console.ResetColor();

            if (summary.FailedInteractions > 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Failed:                {summary.FailedInteractions}");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"Failed:                {summary.FailedInteractions}");
            }

            Console.WriteLine($"Success Rate:          {summary.SuccessRate:F2}%");
            Console.WriteLine(new string('=', 50));

            return summary.FailedInteractions > 0 ? 1 : 0;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\n❌ Error: {ex.Message}");
            Console.WriteLine($"\nStack Trace:\n{ex.StackTrace}");
            Console.ResetColor();
            return 1;
        }
    }

    static TestConfiguration? ParseArguments(string[] args)
    {
        if (args.Length == 0)
        {
            return null;
        }

        // Check if config file is provided
        if (args.Length == 1 && args[0].EndsWith(".json"))
        {
            return LoadConfigFromFile(args[0]);
        }

        var config = new TestConfiguration();

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--url":
                case "-u":
                    if (i + 1 < args.Length)
                        config.BaseUrl = args[++i];
                    break;

                case "--username":
                    if (i + 1 < args.Length)
                        config.Username = args[++i];
                    break;

                case "--password":
                    if (i + 1 < args.Length)
                        config.Password = args[++i];
                    break;

                case "--username-selector":
                    if (i + 1 < args.Length)
                        config.UsernameSelector = args[++i];
                    break;

                case "--password-selector":
                    if (i + 1 < args.Length)
                        config.PasswordSelector = args[++i];
                    break;

                case "--login-button":
                    if (i + 1 < args.Length)
                        config.LoginButtonSelector = args[++i];
                    break;

                case "--max-pages":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var maxPages))
                        config.MaxPagesToCrawl = maxPages;
                    break;

                case "--max-depth":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var maxDepth))
                        config.MaxDepth = maxDepth;
                    break;

                case "--browser":
                    if (i + 1 < args.Length)
                    {
                        config.BrowserType = args[++i].ToLower() switch
                        {
                            "firefox" => BrowserType.Firefox,
                            "webkit" => BrowserType.WebKit,
                            _ => BrowserType.Chromium
                        };
                    }
                    break;

                case "--headless":
                    if (i + 1 < args.Length && bool.TryParse(args[++i], out var headless))
                        config.Headless = headless;
                    break;

                case "--visible":
                    config.Headless = false;
                    break;

                case "--delay":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var delay))
                        config.InteractionDelay = delay;
                    break;

                case "--timeout":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out var timeout))
                        config.NavigationTimeout = timeout;
                    break;

                case "--screenshot-path":
                    if (i + 1 < args.Length)
                        config.ScreenshotPath = args[++i];
                    break;

                case "--no-screenshots":
                    config.ScreenshotOnError = false;
                    break;

                case "--exclude-url":
                    if (i + 1 < args.Length)
                        config.ExcludeUrls.Add(args[++i]);
                    break;

                case "--exclude-login-page":
                    if (i + 1 < args.Length && bool.TryParse(args[++i], out var excludeLogin))
                        config.ExcludeLoginPage = excludeLogin;
                    else
                        config.ExcludeLoginPage = true; // Default to true if no value provided
                    break;

                case "--help":
                case "-h":
                    return null;
            }
        }

        if (string.IsNullOrEmpty(config.BaseUrl))
        {
            Console.WriteLine("Error: Base URL is required.");
            return null;
        }

        return config;
    }

    static TestConfiguration? LoadConfigFromFile(string filePath)
    {
        try
        {
            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) }
            };

            // Deserialize into a new instance
            var config = JsonSerializer.Deserialize<TestConfiguration>(json, options);

            // If config is null, return default
            if (config == null)
            {
                return new TestConfiguration();
            }

            // Create a default config to get default values
            var defaultConfig = new TestConfiguration();

            // Only override non-null string properties
            config.UsernameSelector ??= defaultConfig.UsernameSelector;
            config.PasswordSelector ??= defaultConfig.PasswordSelector;
            config.LoginButtonSelector ??= defaultConfig.LoginButtonSelector;

            return config;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading config file: {ex.Message}");
            return null;
        }
    }

    static async Task SaveResultsAsync(TestSummary summary, string filePath)
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(summary, options);
        await File.WriteAllTextAsync(filePath, json);
    }

    static void ShowUsage()
    {
        Console.WriteLine("Usage:");
        Console.WriteLine("  TestManiac.CLI [options]");
        Console.WriteLine("  TestManiac.CLI <config-file.json>");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -u, --url <url>                Base URL to test (required)");
        Console.WriteLine("  --username <username>          Login username (optional)");
        Console.WriteLine("  --password <password>          Login password (optional)");
        Console.WriteLine("  --username-selector <selector> CSS selector for username field");
        Console.WriteLine("  --password-selector <selector> CSS selector for password field");
        Console.WriteLine("  --login-button <selector>      CSS selector for login button");
        Console.WriteLine("  --max-pages <number>           Maximum pages to crawl (default: 50)");
        Console.WriteLine("  --max-depth <number>           Maximum navigation depth (default: 5)");
        Console.WriteLine("  --browser <type>               Browser type: chromium, firefox, webkit (default: chromium)");
        Console.WriteLine("  --headless <true|false>        Run in headless mode (default: true)");
        Console.WriteLine("  --visible                      Run browser in visible mode (shortcut for --headless false)");
        Console.WriteLine("  --delay <ms>                   Delay between interactions in ms (default: 500)");
        Console.WriteLine("  --timeout <ms>                 Navigation timeout in ms (default: 30000)");
        Console.WriteLine("  --screenshot-path <path>       Path to save screenshots (default: ./screenshots)");
        Console.WriteLine("  --no-screenshots               Disable screenshots on errors");
        Console.WriteLine("  --exclude-url <pattern>        URL pattern to exclude (can be used multiple times)");
        Console.WriteLine("  --exclude-login-page [bool]    Exclude login page from testing (default: true)");
        Console.WriteLine("  -h, --help                     Show this help message");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  TestManiac.CLI --url https://example.com --visible");
        Console.WriteLine();
        Console.WriteLine("  TestManiac.CLI --url https://example.com \\");
        Console.WriteLine("    --username admin --password secret123 \\");
        Console.WriteLine("    --username-selector \"#username\" \\");
        Console.WriteLine("    --password-selector \"#password\" \\");
        Console.WriteLine("    --login-button \"button[type='submit']\"");
        Console.WriteLine();
        Console.WriteLine("  TestManiac.CLI config.json");
    }
}
