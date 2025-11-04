# TestManiac - Project Overview

## What is TestManiac?

TestManiac is an automated web testing library and tool that systematically tests your web application by:

- Opening your website in a real browser
- Logging in (if credentials provided)
- Clicking every interactive element (buttons, links, etc.)
- Detecting errors after each interaction
- Crawling through all pages within your domain
- Generating comprehensive reports with screenshots

## Architecture

### Project Structure

```
TestManiac/
│
├── TestManiac.Core/              # Reusable Class Library (.NET 8)
│   ├── TestConfiguration.cs      # Configuration model with all settings
│   ├── TestResult.cs             # Result models (TestResult, TestSummary)
│   └── WebTester.cs              # Main testing engine with Playwright
│
├── TestManiac.CLI/               # Command-Line Interface (.NET 8)
│   └── Program.cs                # CLI wrapper for easy usage
│
├── Documentation/
│   ├── README.md                 # Complete user guide
│   ├── SETUP.md                  # Installation and setup
│   ├── QUICKSTART.md             # Quick start examples
│   └── PROJECT_OVERVIEW.md       # This file
│
├── Configuration Examples/
│   ├── config.example.json       # Basic configuration
│   └── config.with-login.example.json  # Configuration with login
│
├── Scripts/
│   ├── setup.ps1                 # Automated setup script
│   └── run-test.bat              # Quick test runner for Windows
│
└── TestManiac.sln                # Visual Studio Solution

```

## Core Components

### 1. TestConfiguration Class

Holds all configuration parameters:

- URL and domain settings (baseUrl, startUrl)
- Login credentials and selectors
- Browser configuration (type, headless mode)
- Crawling limits (max pages, max depth)
- Timing settings (timeouts, delays, click timeout, network idle timeout)
- Screenshot settings (path, on error)
- Results output path
- Network idle detection settings
- Dialog handler action (accept, dismiss, ignore)
- Auto-close modal settings (enabled, dialog selectors, close selectors)
- URL exclusion patterns
- SSL certificate error handling

### 2. WebTester Class

The main testing engine that:

- Initializes Playwright and browser
- Handles login flow
- Navigates to start URL (if specified)
- Crawls pages recursively
- Identifies interactive elements
- Performs interactions safely (with JavaScript fallback)
- Waits for network idle after clicks
- Detects errors on pages
- Captures before/after combined screenshots
- Tracks visited URLs to avoid duplicates
- Navigates back after following links
- Handles URL exclusion patterns

**Key Methods:**

- `InitializeAsync()` - Sets up browser
- `RunTestsAsync()` - Main test execution
- `PerformLoginAsync()` - Handles authentication
- `CrawlPageAsync()` - Recursive page crawler
- `GetInteractableElementsAsync()` - Finds clickable elements
- `CheckForErrorsAsync()` - Error detection
- `TakeScreenshotAsync()` - Captures screenshots with element highlighting
- `CombineScreenshots()` - Creates before/after comparison images
- `WaitForNetworkIdleAsync()` - Waits for background requests to complete
- `ShouldExcludeUrl()` - Checks URL exclusion patterns

**Events:**

- `LogMessage` - Real-time logging
- `InteractionCompleted` - After each interaction
- `PageVisited` - When navigating to new page

### 3. TestResult & TestSummary Classes

Data models for test results:

- Individual interaction results
- Error messages and types
- Screenshot paths
- Aggregate statistics
- Success rates

### 4. CLI Program

Command-line interface that:

- Parses arguments or config files
- Displays progress in real-time
- Shows formatted summary
- Saves results to JSON
- Returns appropriate exit codes

## How It Works

### Execution Flow

```
1. Parse Configuration
   ↓
2. Initialize Playwright & Browser
   ↓
3. Navigate to Base URL
   ↓
4. Perform Login (if configured)
   ↓
5. Navigate to Start URL (if specified)
   ↓
6. Start Crawling Current Page
   │
   ├─→ Find All Interactive Elements
   │   │
   │   ├─→ Take "Before" Screenshot (element highlighted)
   │   ├─→ Click Element (with JS fallback if overlapped)
   │   ├─→ Wait for Network Idle
   │   ├─→ Check for Errors
   │   ├─→ Take "After" Screenshot (if error)
   │   ├─→ Combine Screenshots (if error)
   │   ├─→ Navigate Back (if page changed)
   │   └─→ Repeat for next element
   │
   ├─→ Collect Links to Same Domain (excluding patterns)
   │
   └─→ Recursively Crawl Each Link
       (respecting max depth & max pages)
   ↓
7. Generate Summary Report
   ↓
8. Save Results to JSON
   ↓
9. Display Statistics & Exit
```

### Element Interaction Logic

```csharp
// Finds elements matching these patterns:
- button:visible:not([disabled])
- a:visible:not([disabled])
- input[type='button']:visible:not([disabled])
- input[type='submit']:visible:not([disabled])
- [role='button']:visible:not([disabled])
- [onclick]:visible:not([disabled])

// For each element:
1. Verify it's visible and enabled
2. Get element description (tag, text, or outerHTML if no text)
3. Take "before" screenshot with element highlighted
4. Try to click element with timeout
   - If overlapped, automatically retry with JavaScript click
5. Wait for network idle (background API calls)
6. Check for error indicators
7. If error found:
   - Take "after" screenshot
   - Combine before/after into side-by-side image
8. If page navigated, go back
9. Record result with element description and URL
```

### Error Detection

The tool looks for common error patterns:

```csharp
- [class*='error']:visible
- [class*='alert-danger']:visible
- [role='alert']:visible
- .error-message:visible
- #error:visible
```

It also monitors:

- JavaScript console errors
- Page errors (unhandled exceptions)
- HTTP error status codes (future enhancement)

## Technology Stack

- **Language**: C# 12 / .NET 8.0
- **Browser Automation**: Microsoft Playwright
- **Browsers Supported**: Chromium, Firefox, WebKit
- **Serialization**: System.Text.Json
- **Target Platforms**: Windows, macOS, Linux

## Key Features

### ✅ Implemented

1. **Browser Automation**

   - Multi-browser support (Chromium, Firefox, WebKit)
   - Headless and visible modes
   - Configurable timeouts and delays

2. **Intelligent Crawling**

   - Domain-aware (stays within same domain)
   - Recursive navigation with depth limits
   - Visited URL tracking (avoids duplicates)
   - Smart back navigation after link clicks

3. **Login Support**

   - Optional authentication
   - Configurable selectors
   - Form filling and submission

4. **Element Interaction**

   - Auto-discovery of interactive elements
   - Visibility and enabled state verification
   - Safe click with error handling
   - JavaScript click fallback for overlapped elements
   - Recovery from failures

5. **Error Detection**

   - Multiple error pattern matching
   - Console error monitoring
   - Page error tracking
   - Before/after combined screenshot capture
   - Element highlighting in screenshots

6. **JavaScript Dialog Handling**

   - Automatic accept/dismiss of alerts
   - Configurable confirm dialog responses
   - Prompt handling
   - Prevents test blocking on dialogs

7. **HTML Modal Dialog Handling**

   - Auto-close HTML modals (Bootstrap, custom, etc.)
   - Configurable close button selectors
   - Prevents blocking on popup dialogs
   - Continues testing after modal dismissal

8. **URL Management**

   - Domain-aware navigation
   - URL exclusion patterns (wildcards supported)
   - Exclude login page option
   - Start URL configuration

9. **Reporting**

   - Real-time console output with detailed element info
   - Detailed JSON reports
   - Success/failure statistics
   - Combined before/after screenshot gallery

10. **Configuration**

- Command-line arguments
- JSON configuration files
- Flexible parameter options
- Network idle detection settings
- Dialog handler configuration
- Modal auto-close settings

### 🔮 Future Enhancements

Potential additions:

- Form filling and submission testing
- Custom element selectors for interaction
- Network request monitoring
- Performance metrics
- HTML report generation
- CI/CD integration helpers
- Parallel browser execution
- Custom error detection rules
- API endpoint testing
- Accessibility checks

## Use Cases

### 1. Smoke Testing

Quick verification that basic functionality works after deployment:

```bash
dotnet run --project TestManiac.CLI -- --url https://prod.yoursite.com --max-pages 20
```

### 2. Regression Testing

Verify no UI errors introduced in new releases:

```bash
dotnet run --project TestManiac.CLI -- config-regression.json
```

### 3. Exploratory Testing

Discover edge cases and unexpected behaviors:

```bash
dotnet run --project TestManiac.CLI -- --url https://test.yoursite.com --max-depth 5 --visible
```

### 4. Integration Testing

Test after logging in as different user types:

```bash
dotnet run --project TestManiac.CLI -- config-admin-user.json
dotnet run --project TestManiac.CLI -- config-regular-user.json
```

### 5. Custom Automation

Use the library in your own test projects:

```csharp
var tester = new WebTester(config);
tester.InteractionCompleted += MyCustomHandler;
await tester.RunTestsAsync();
```

## Team Usage

### For Developers

- Reference `TestManiac.Core` in your test projects
- Extend `WebTester` class for custom behavior
- Subscribe to events for custom logging/reporting

### For QA Engineers

- Use CLI with various configurations
- Create JSON configs for different test scenarios
- Analyze JSON reports and screenshots
- Integrate into test automation suites

### For DevOps/CI

- Run in headless mode in pipelines
- Parse JSON output for pass/fail status
- Archive screenshots as artifacts
- Use exit codes for pipeline decisions

## Performance Considerations

### Typical Performance

- **Small site** (10 pages, 50 elements): 2-5 minutes
- **Medium site** (50 pages, 200 elements): 10-20 minutes
- **Large site** (100+ pages): Limit with max-pages and max-depth

### Optimization Tips

1. Use `--max-pages` and `--max-depth` to limit scope
2. Increase `--delay` if site is slow to respond
3. Use `--headless true` for faster execution
4. Run in parallel for different site sections (multiple processes)

## Troubleshooting Guide

### Common Issues

**Issue**: "Playwright browsers not installed"
**Solution**: Run `setup.ps1` or manually install browsers

**Issue**: Too many pages being crawled
**Solution**: Reduce `--max-pages` and `--max-depth`

**Issue**: Timeout errors
**Solution**: Increase `--timeout` value

**Issue**: Elements not found
**Solution**: Increase `--delay` or check element selectors

**Issue**: False positive errors
**Solution**: Review error detection selectors, may need customization

## Maintenance

### Updating Dependencies

```bash
cd TestManiac.Core
dotnet add package Microsoft.Playwright --version [latest]
cd ../TestManiac.CLI
dotnet build
```

### Running Tests

Currently no unit tests included. Recommended additions:

- Unit tests for configuration parsing
- Mock tests for WebTester methods
- Integration tests with test pages

### Code Quality

- Follow .NET coding standards
- Add XML documentation comments
- Use async/await consistently
- Handle exceptions gracefully
- Log important events

## Support & Contribution

### Getting Help

1. Check [QUICKSTART.md](QUICKSTART.md)
2. Review [README.md](README.md)
3. Examine example configurations
4. Run with `--visible` to observe behavior
5. Contact your automation engineer

### Contributing

To add features or fix bugs:

1. Create a feature branch
2. Make changes with clear commit messages
3. Test thoroughly
4. Update documentation
5. Submit for review

## License

MIT License - See [LICENSE](LICENSE) file

---

**Built for automation engineers who want reliable, automated testing of web applications.**
