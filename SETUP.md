# Setup Instructions

## Initial Setup

Follow these steps to set up TestManiac on your machine:

### 1. Prerequisites

Ensure you have .NET 8.0 SDK installed:

```bash
dotnet --version
```

If not installed, download from: https://dotnet.microsoft.com/download/dotnet/8.0

### 2. Build the Project

```bash
cd C:\Projects\TestManiac
dotnet build
```

### 3. Install Playwright Browsers

**IMPORTANT**: You must install Playwright browsers before running tests.

#### On Windows (PowerShell):

```powershell
cd C:\Projects\TestManiac
pwsh TestManiac.Core\bin\Debug\net8.0\playwright.ps1 install
```

If you get an execution policy error, run:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

#### On Linux/macOS:

```bash
cd /path/to/TestManiac
bash TestManiac.Core/bin/Debug/net8.0/playwright.sh install
```

### 4. Verify Installation

Run a simple test:

```bash
cd C:\Projects\TestManiac
dotnet run --project TestManiac.CLI -- --url https://example.com --max-pages 1 --visible
```

## Building for Distribution

To create a standalone executable that teammates can use:

### Option 1: Single-File Executable (Recommended)

```bash
# For Windows
dotnet publish TestManiac.CLI/TestManiac.CLI.csproj -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -o ./publish

# For macOS
dotnet publish TestManiac.CLI/TestManiac.CLI.csproj -c Release -r osx-x64 --self-contained -p:PublishSingleFile=true -o ./publish

# For Linux
dotnet publish TestManiac.CLI/TestManiac.CLI.csproj -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -o ./publish
```

The executable will be in the `publish` folder. Share this folder with your teammates.

**Note**: Recipients will still need to run the Playwright browser installation command once:

```bash
# Windows
cd publish
pwsh playwright.ps1 install

# Linux/macOS
cd publish
bash playwright.sh install
```

### Option 2: Framework-Dependent (Smaller size)

```bash
dotnet publish TestManiac.CLI/TestManiac.CLI.csproj -c Release -o ./publish
```

Recipients need .NET 8.0 Runtime installed.

## Using the Library in Your Own Project

1. Add a reference to TestManiac.Core:

```bash
dotnet add reference path/to/TestManiac.Core/TestManiac.Core.csproj
```

2. Add Playwright package to your project:

```bash
dotnet add package Microsoft.Playwright
```

3. Install Playwright browsers (one-time):

```bash
pwsh bin/Debug/net8.0/playwright.ps1 install
```

4. Use in your code:

```csharp
using TestManiac.Core;

var config = new TestConfiguration
{
    BaseUrl = "https://yoursite.com",
    MaxPagesToCrawl = 10
};

await using var tester = new WebTester(config);
var summary = await tester.RunTestsAsync();
```

## Troubleshooting

### "Playwright browsers are not installed"

Run the installation command from step 3 above.

### "dotnet command not found"

Install .NET SDK from: https://dotnet.microsoft.com/download

### PowerShell script execution blocked

Run:

```powershell
Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
```

### Browser crashes or hangs

Try running with increased timeout:

```bash
dotnet run --project TestManiac.CLI -- --url https://example.com --timeout 60000
```

## Next Steps

1. Read [README.md](README.md) for usage instructions
2. Check [config.example.json](config.example.json) for configuration options
3. Start with a small test to understand the tool behavior
4. Gradually increase `--max-pages` and `--max-depth` as needed

## Updating

When code changes are made:

```bash
cd C:\Projects\TestManiac
git pull  # If using version control
dotnet build
```

Browser reinstallation is usually not needed after code updates.
