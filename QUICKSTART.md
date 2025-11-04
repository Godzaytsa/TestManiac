# Quick Start Guide

## For First-Time Users

### Step 1: Install Playwright Browsers (One-Time Setup)

Open PowerShell in the TestManiac directory and run:

```powershell
cd C:\Projects\TestManiac\TestManiac.CLI\bin\Debug\net8.0
powershell -File playwright.ps1 install
```

This downloads the browser engines needed for testing. Only needs to be done once.

### Step 2: Run Your First Test

```bash
# Navigate to project root
cd C:\Projects\TestManiac

# Run a simple test (visible browser)
dotnet run --project TestManiac.CLI -- --url https://example.com --max-pages 5 --visible
```

## Common Commands

### Test a public website (no login)

```bash
dotnet run --project TestManiac.CLI -- --url https://example.com --visible
```

### Test with limited scope

```bash
dotnet run --project TestManiac.CLI -- --url https://yoursite.com --max-pages 10 --max-depth 2
```

### Test with login and start from specific page

```bash
dotnet run --project TestManiac.CLI -- ^
  --url https://yoursite.com ^
  --start-url https://yoursite.com/dashboard ^
  --username your-username ^
  --password your-password ^
  --username-selector "#username" ^
  --password-selector "#password" ^
  --login-button "button[type='submit']" ^
  --visible
```

### Test with SSL certificate errors ignored (self-signed certs)

```bash
dotnet run --project TestManiac.CLI -- ^
  --url https://test.yoursite.com ^
  --ignore-ssl-errors ^
  --visible
```

### Test with custom modal handling

```bash
dotnet run --project TestManiac.CLI -- ^
  --url https://yoursite.com ^
  --auto-close-modals ^
  --modal-dialog-selector ".custom-modal.show" ^
  --modal-close-selector ".custom-close-btn" ^
  --visible
```

### Test with URL exclusions

```bash
dotnet run --project TestManiac.CLI -- ^
  --url https://yoursite.com ^
  --exclude-url "/admin/*" ^
  --exclude-url "/api/*" ^
  --exclude-login-page ^
  --visible
```

### Test with element exclusions (avoid clicking logout, delete, etc.)

```bash
dotnet run --project TestManiac.CLI -- ^
  --url https://yoursite.com ^
  --exclude-element-pattern "(?i)log.*out" ^
  --exclude-element-pattern "/logout" ^
  --exclude-element-pattern "(?i)(delete|remove)" ^
  --visible
```

### Headless mode (no browser window)

```bash
dotnet run --project TestManiac.CLI -- --url https://example.com --headless true
```

### Use a config file

```bash
# Create your config file (copy from config.example.json)
copy config.example.json mytest.json

# Edit mytest.json with your settings

# Run with config
dotnet run --project TestManiac.CLI -- mytest.json
```

## Tips

1. **Always start with `--visible`** so you can see what's happening
2. **Start small** - Use `--max-pages 5 --max-depth 2` initially
3. **Organize results** - Use `--results-path ./results` to save files in a dedicated folder
4. **Handle dialogs** - Use `--dialog-handler accept` to automatically accept alerts/confirms (default)
5. **Auto-close modals** - Use `--auto-close-modals` to prevent HTML modal dialogs from blocking tests
6. **SSL issues** - Use `--ignore-ssl-errors` for sites with self-signed or invalid certificates
7. **Network timing** - Use `--wait-network-idle` (default: true) to wait for API calls before checking errors
8. **Exclude elements** - Use `--exclude-element-pattern "(?i)log.*out"` to avoid clicking logout/delete buttons
9. **Check the results** - Look at the generated `test-results_*.json` file
10. **Screenshots** - Check the `screenshots/` folder if errors occur
11. **Get help** - Run `dotnet run --project TestManiac.CLI -- --help`

## Finding CSS Selectors for Login

1. Open your website in Chrome/Edge
2. Right-click on the username field → Inspect
3. In DevTools, right-click the highlighted element
4. Copy → Copy selector
5. Use that selector with `--username-selector`
6. Repeat for password field and login button

Example:

- Username field: `#username` or `input[name='username']`
- Password field: `#password` or `input[name='password']`
- Login button: `button[type='submit']` or `#login-button`

## What to Expect

The tool will:

1. Open a browser
2. Navigate to your URL
3. Login (if credentials provided)
4. Click every button and link it finds
5. Check for errors after each click
6. Navigate through pages within your domain
7. Generate a report when done

## Output Files

- `test-results_TIMESTAMP.json` - Detailed results (saved to current directory or `resultsPath` if specified)
- `screenshots/error_*.png` - Combined before/after screenshots of errors (side-by-side comparison)
- Console output - Real-time progress

## Need Help?

Check:

- [README.md](README.md) - Full documentation
- [SETUP.md](SETUP.md) - Detailed setup instructions
- [config.example.json](config.example.json) - Configuration examples
