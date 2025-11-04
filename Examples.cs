using TestManiac.Core;

// Example 1: Basic usage without login
Console.WriteLine("Example 1: Basic Test\n");

var basicConfig = new TestConfiguration
{
    BaseUrl = "https://example.com",
    MaxPagesToCrawl = 10,
    MaxDepth = 2,
    Headless = false, // Set to true for headless mode
    BrowserType = BrowserType.Chromium
};

await using (var tester = new WebTester(basicConfig))
{
    // Subscribe to events for logging
    tester.LogMessage += (sender, message) => Console.WriteLine($"[LOG] {message}");

    tester.PageVisited += (sender, url) => Console.WriteLine($"[PAGE] Visiting: {url}");

    tester.InteractionCompleted += (sender, result) =>
    {
        if (result.Success)
        {
            Console.WriteLine($"[OK] {result.ElementDescription}");
        }
        else
        {
            Console.WriteLine($"[ERROR] {result.ElementDescription}: {result.ErrorMessage}");
        }
    };

    // Run tests
    var summary = await tester.RunTestsAsync();

    // Display results
    Console.WriteLine("\n--- Summary ---");
    Console.WriteLine($"Duration: {summary.Duration}");
    Console.WriteLine($"Pages: {summary.TotalPagesVisited}");
    Console.WriteLine($"Interactions: {summary.TotalInteractions}");
    Console.WriteLine($"Success Rate: {summary.SuccessRate:F2}%");
}

Console.WriteLine("\n" + new string('=', 50) + "\n");

// Example 2: With login
Console.WriteLine("Example 2: Test with Login\n");

var loginConfig = new TestConfiguration
{
    BaseUrl = "https://your-app.com/login",
    Username = "testuser@example.com",
    Password = "TestPassword123",
    UsernameSelector = "input[name='email']",
    PasswordSelector = "input[name='password']",
    LoginButtonSelector = "button[type='submit']",
    MaxPagesToCrawl = 20,
    MaxDepth = 3,
    Headless = true,
    InteractionDelay = 1000 // 1 second delay between interactions
};

await using (var tester = new WebTester(loginConfig))
{
    tester.LogMessage += (sender, message) => Console.WriteLine(message);

    var summary = await tester.RunTestsAsync();

    // Check for failures
    if (summary.FailedInteractions > 0)
    {
        Console.WriteLine("\nFailed Interactions:");
        foreach (var result in summary.Results.Where(r => !r.Success))
        {
            Console.WriteLine($"  - {result.ElementDescription}");
            Console.WriteLine($"    URL: {result.Url}");
            Console.WriteLine($"    Error: {result.ErrorMessage}");
            if (!string.IsNullOrEmpty(result.ScreenshotPath))
            {
                Console.WriteLine($"    Screenshot: {result.ScreenshotPath}");
            }
        }
    }
}

Console.WriteLine("\n" + new string('=', 50) + "\n");

// Example 3: Custom configuration from JSON
Console.WriteLine("Example 3: Load from JSON Config\n");

var jsonConfig = """
{
  "baseUrl": "https://example.com",
  "maxPagesToCrawl": 15,
  "maxDepth": 2,
  "headless": true,
  "browserType": "Firefox",
  "screenshotOnError": true,
  "screenshotPath": "./my-screenshots"
}
""";

var config = System.Text.Json.JsonSerializer.Deserialize<TestConfiguration>(jsonConfig,
    new System.Text.Json.JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

if (config != null)
{
    await using var tester = new WebTester(config);
    tester.LogMessage += (sender, message) => Console.WriteLine(message);

    var summary = await tester.RunTestsAsync();

    Console.WriteLine($"\n✓ Test completed with {summary.SuccessRate:F2}% success rate");
}

Console.WriteLine("\nAll examples completed!");
