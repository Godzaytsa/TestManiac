# TestManiac

**TestManiac** is a powerful automated web testing tool built with C# and Playwright. It automatically crawls your website, interacts with all visible and enabled elements, and reports any errors that occur. Perfect for smoke testing, exploratory testing, and catching UI errors across your entire web application.

## Features

✅ **Automated Element Interaction** - Automatically clicks all buttons, links, and interactive elements  
✅ **Domain-Aware Crawling** - Stays within your domain while navigating through pages  
✅ **Smart Navigation** - Automatically returns to previous pages after following links  
✅ **Error Detection** - Identifies and reports errors after each interaction  
✅ **Before/After Screenshots** - Captures combined screenshots showing element before click and page after error  
✅ **JavaScript Click Fallback** - Automatically retries with JS click when elements are overlapped  
✅ **Network Idle Detection** - Waits for background API calls to complete before checking for errors  
✅ **Dialog Handler** - Automatically accepts, dismisses, or ignores JavaScript alerts/confirms/prompts  
✅ **Auto-Close Modals** - Automatically closes HTML modal dialogs to prevent blocking  
✅ **Login Support** - Optional login with configurable credentials  
✅ **Multiple Browsers** - Supports Chromium, Firefox, and WebKit  
✅ **Detailed Reports** - JSON output with comprehensive test results  
✅ **Flexible Configuration** - Command-line arguments or JSON config files  
✅ **URL Exclusion** - Skip specific pages or patterns from testing  
✅ **Reusable Library** - Use the core library in your own projects

## Installation

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.net/download/dotnet/8.0) or later
- Windows, macOS, or Linux

### Setup

1. **Clone or download this repository**

2. **Restore packages:**

   ```bash
   cd TestManiac
   dotnet restore
   ```

3. **Install Playwright browsers:**

   ```bash
   cd TestManiac.Core
   pwsh bin/Debug/net8.0/playwright.ps1 install
   ```

   Or on Linux/macOS:

   ```bash
   cd TestManiac.Core
   bash bin/Debug/net8.0/playwright.sh install
   ```

4. **Build the solution:**

   ```bash
   cd ..
   dotnet build
   ```

5. **Publish the CLI (Optional - for standalone executable):**
   ```bash
   dotnet publish TestManiac.CLI/TestManiac.CLI.csproj -c Release -o ./publish
   ```

## Usage

### Quick Start

Test a website without login:

```bash
dotnet run --project TestManiac.CLI -- --url https://example.com --visible
```

### Command Line Options

```
TestManiac.CLI [options]
TestManiac.CLI <config-file.json>

Options:
  -u, --url <url>                Base URL to test (required)
  -s, --start-url <url>          Start testing from this URL (optional)
  --username <username>          Login username (optional)
  --password <password>          Login password (optional)
  --username-selector <selector> CSS selector for username field
  --password-selector <selector> CSS selector for password field
  --login-button <selector>      CSS selector for login button
  --max-pages <number>           Maximum pages to crawl (default: 50)
  --max-depth <number>           Maximum navigation depth (default: 5)
  --browser <type>               Browser type: chromium, firefox, webkit (default: chromium)
  --headless <true|false>        Run in headless mode (default: true)
  --visible                      Run browser in visible mode
  --delay <ms>                   Delay between interactions in ms (default: 500)
  --timeout <ms>                 Navigation timeout in ms (default: 30000)
  --click-timeout <ms>           Click timeout in ms (default: 10000)
  --wait-network-idle <bool>     Wait for network idle after clicks (default: true)
  --network-idle-timeout <ms>    Network idle timeout in ms (default: 10000)
  --screenshot-path <path>       Path to save screenshots (default: ./screenshots)
  --results-path <path>          Path to save test results JSON (default: current directory)
  --dialog-handler <action>      How to handle dialogs: accept, dismiss, ignore (default: accept)
  --auto-close-modals [bool]     Automatically close HTML modal dialogs (default: false)
  --modal-dialog-selector <sel>  CSS selector for modal dialog containers (can be used multiple times)
  --modal-close-selector <sel>   CSS selector for modal close buttons (can be used multiple times)
  --exclude-url <pattern>        URL pattern to exclude (can be used multiple times)
  --exclude-login-page [bool]    Exclude login page from testing (default: true)
  --ignore-ssl-errors [bool]     Ignore SSL certificate errors (default: false)
  --no-screenshots               Disable screenshots on errors
  -h, --help                     Show help message
```

### Examples

#### Basic Testing

```bash
# Test a website in visible mode
dotnet run --project TestManiac.CLI -- --url https://example.com --visible

# Test with limited pages and depth
dotnet run --project TestManiac.CLI -- --url https://example.com --max-pages 20 --max-depth 3

# Save results to a specific folder
dotnet run --project TestManiac.CLI -- --url https://example.com --results-path "./test-results" --visible
```

#### Testing with Login

```bash
dotnet run --project TestManiac.CLI -- \
  --url https://example.com \
  --username admin \
  --password secret123 \
  --username-selector "#username" \
  --password-selector "#password" \
  --login-button "button[type='submit']" \
  --visible
```

#### Using Different Browsers

```bash
# Use Firefox
dotnet run --project TestManiac.CLI -- --url https://example.com --browser firefox

# Use WebKit (Safari engine)
dotnet run --project TestManiac.CLI -- --url https://example.com --browser webkit
```

#### Using Configuration File

```bash
# Create a config file (see config.example.json)
dotnet run --project TestManiac.CLI -- config.json
```

### Configuration File Format

Create a JSON file with your test configuration:

```json
{
  "baseUrl": "https://example.com",
  "startUrl": "https://example.com/dashboard",
  "username": "admin",
  "password": "secret123",
  "usernameSelector": "#username",
  "passwordSelector": "#password",
  "loginButtonSelector": "button[type='submit']",
  "maxPagesToCrawl": 50,
  "maxDepth": 5,
  "navigationTimeout": 30000,
  "headless": true,
  "browserType": "Chromium",
  "interactionDelay": 500,
  "clickTimeout": 5000,
  "waitForNetworkIdle": true,
  "networkIdleTimeout": 10000,
  "screenshotOnError": true,
  "screenshotPath": "./screenshots",
  "resultsPath": "./test-results",
  "dialogHandler": "Accept",
  "autoCloseModals": false,
  "modalDialogSelectors": [".modal.show", "[role='dialog'][aria-modal='true']"],
  "modalCloseSelectors": [
    "button.close",
    "[data-dismiss='modal']",
    ".modal-close"
  ],
  "excludeUrls": ["/logout", "/admin/*"],
  "excludeLoginPage": true,
  "ignoreSslErrors": false
}
```

Then run:

```bash
dotnet run --project TestManiac.CLI -- myconfig.json
```

## Using as a Library

You can use `TestManiac.Core` in your own C# projects:

### Add Reference

```bash
dotnet add reference path/to/TestManiac.Core/TestManiac.Core.csproj
```

### Example Code

```csharp
using TestManiac.Core;

var config = new TestConfiguration
{
    BaseUrl = "https://example.com",
    Headless = false,
    MaxPagesToCrawl = 20,
    MaxDepth = 3
};

await using var tester = new WebTester(config);

// Subscribe to events
tester.LogMessage += (sender, message) => Console.WriteLine(message);
tester.InteractionCompleted += (sender, result) =>
{
    if (!result.Success)
    {
        Console.WriteLine($"Failed: {result.ElementDescription}");
    }
};

// Run tests
var summary = await tester.RunTestsAsync();

Console.WriteLine($"Total Interactions: {summary.TotalInteractions}");
Console.WriteLine($"Success Rate: {summary.SuccessRate:F2}%");
```

## How It Works

1. **Initialization**: Launches the specified browser and navigates to the base URL
2. **Login** (Optional): If credentials are provided, performs login
3. **Navigation**: If startUrl is specified, navigates to that page to begin testing
4. **Crawling**: Starting from the start page (or base page):
   - Identifies all visible and enabled interactive elements (buttons, links, etc.)
   - Takes a "before" screenshot with the element highlighted
   - Clicks each element one by one (with JavaScript fallback if element is overlapped)
   - Waits for network to become idle (all API calls complete)
   - Checks for errors after each interaction
   - If error detected, takes "after" screenshot and combines both into side-by-side comparison
   - If a link leads to a new page within the same domain, crawls that page recursively
   - Returns to the previous page after exploring links
5. **Reporting**: Generates a comprehensive JSON report with all interactions and results

## Output

### Console Output

Real-time progress updates showing:

- Pages being visited
- Elements being interacted with
- Success/failure status
- Error messages
- Final summary with statistics

### JSON Report

A detailed JSON file (`test-results_TIMESTAMP.json`) containing:

- Test duration
- All visited URLs
- Every interaction performed
- Success/failure status for each interaction
- Error messages and types
- Screenshot paths
- Overall statistics and success rate

### Screenshots

When errors occur, the tool captures:

- **Before screenshot**: Shows the page with the clicked element highlighted in red
- **After screenshot**: Shows the page state after the error occurred
- **Combined screenshot**: Both images side-by-side with "BEFORE" and "AFTER" labels

Screenshots are saved to the specified directory with descriptive filenames.

## Best Practices

1. **Start Small**: Begin with a low `--max-pages` and `--max-depth` to understand the behavior
2. **Use Visible Mode**: Run with `--visible` flag during initial testing to see what's happening
3. **Adjust Delays**: Increase `--delay` for slower websites or complex interactions
4. **Domain Scope**: The tool only crawls pages within the same domain as the base URL
5. **Login Selectors**: Use browser DevTools to find the correct CSS selectors for login fields
6. **Organize Results**: Use `--results-path` to save test results in a dedicated folder
7. **Review Results**: Always check the JSON report and screenshots for detailed analysis

## Troubleshooting

### Playwright Not Found

Run the Playwright installation command:

```bash
pwsh TestManiac.Core/bin/Debug/net8.0/playwright.ps1 install
```

### Too Many Pages

Reduce `--max-pages` and `--max-depth` values:

```bash
--max-pages 10 --max-depth 2
```

### Elements Not Found

Increase the `--timeout` value:

```bash
--timeout 60000
```

### Login Issues

- Verify CSS selectors using browser DevTools
- Try running with `--visible` to watch the login process
- Ensure login fields are visible before the tool tries to interact with them

## Project Structure

```
TestManiac/
├── TestManiac.Core/          # Core library
│   ├── TestConfiguration.cs  # Configuration model
│   ├── TestResult.cs         # Result models
│   └── WebTester.cs          # Main testing logic
├── TestManiac.CLI/           # Console application
│   └── Program.cs            # CLI interface
├── config.example.json       # Example configuration
└── README.md                 # This file
```

## Contributing

This is a tool for your team. Feel free to:

- Add new features
- Improve error detection
- Enhance reporting
- Support additional browsers
- Add more configuration options

## License

This tool is provided as-is for your team's internal use.

## Support

For questions or issues:

1. Check this README
2. Review the example configuration
3. Run with `--visible` flag to observe behavior
4. Check the generated JSON reports
5. Reach out to your team's automation engineer

---

**Happy Testing! 🚀**
