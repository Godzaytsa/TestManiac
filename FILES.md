# TestManiac - Files Overview

## Quick Reference

This document provides a quick overview of all files in the TestManiac project.

## Solution Files

| File             | Purpose                     |
| ---------------- | --------------------------- |
| `TestManiac.sln` | Visual Studio solution file |
| `.gitignore`     | Git ignore patterns         |
| `LICENSE`        | MIT License                 |

## Documentation

| File                  | Description            | When to Use                                  |
| --------------------- | ---------------------- | -------------------------------------------- |
| `README.md`           | Complete documentation | Comprehensive guide to all features          |
| `QUICKSTART.md`       | Quick start guide      | First-time users wanting immediate examples  |
| `SETUP.md`            | Installation guide     | Setting up for first time or troubleshooting |
| `PROJECT_OVERVIEW.md` | Architecture & design  | Understanding how it works internally        |
| `FILES.md`            | This file              | Finding specific files                       |

## Configuration Examples

| File                             | Description                          |
| -------------------------------- | ------------------------------------ |
| `config.example.json`            | Basic configuration without login    |
| `config.with-login.example.json` | Configuration with login credentials |

## Scripts

| File           | Platform           | Purpose                                  |
| -------------- | ------------------ | ---------------------------------------- |
| `setup.ps1`    | Windows PowerShell | Automated setup and browser installation |
| `run-test.bat` | Windows Batch      | Quick test runner                        |

## Core Library (`TestManiac.Core/`)

| File                     | Description                             |
| ------------------------ | --------------------------------------- |
| `TestConfiguration.cs`   | Configuration model with all settings   |
| `TestResult.cs`          | Result models (TestResult, TestSummary) |
| `WebTester.cs`           | Main testing engine (400+ lines)        |
| `TestManiac.Core.csproj` | Project file with dependencies          |

### Key Classes

**TestConfiguration**

- Properties: BaseUrl, Username, Password, BrowserType, etc.
- Configures all aspects of testing

**WebTester**

- Main testing orchestrator
- Methods: InitializeAsync(), RunTestsAsync(), CrawlPageAsync()
- Events: LogMessage, InteractionCompleted, PageVisited

**TestResult**

- Individual interaction result
- Properties: Url, ElementDescription, Success, ErrorMessage, ScreenshotPath

**TestSummary**

- Aggregate test results
- Properties: TotalPagesVisited, TotalInteractions, SuccessRate, Results list

## CLI Application (`TestManiac.CLI/`)

| File                    | Description                           |
| ----------------------- | ------------------------------------- |
| `Program.cs`            | Command-line interface implementation |
| `TestManiac.CLI.csproj` | Project file                          |

### Key Functions

**Main()**

- Entry point, orchestrates CLI flow
- Parses arguments, runs tests, displays results

**ParseArguments()**

- Converts command-line args to TestConfiguration
- Handles both CLI args and JSON config files

**ShowUsage()**

- Displays help text

## Generated Files (Not in Source Control)

### Build Outputs

```
TestManiac.Core/bin/Debug/net8.0/
  ├── TestManiac.Core.dll
  ├── Microsoft.Playwright.dll
  └── playwright.ps1

TestManiac.CLI/bin/Debug/net8.0/
  ├── TestManiac.CLI.exe       # Windows executable
  ├── TestManiac.CLI.dll
  ├── TestManiac.Core.dll
  ├── Microsoft.Playwright.dll
  └── playwright.ps1
```

### Runtime Generated

```
screenshots/                    # Error screenshots
  ├── error_0_20251104_120530.png
  └── error_1_20251104_120545.png

test-results_20251104_120000.json  # Test results
```

## Typical Workflow Files

### Developer Workflow

1. `SETUP.md` - Initial setup
2. `TestManiac.Core/WebTester.cs` - Modify/extend logic
3. `TestManiac.CLI/Program.cs` - Update CLI options
4. `README.md` - Update documentation

### User Workflow

1. `QUICKSTART.md` - Learn basics
2. `config.example.json` - Create config
3. Run CLI tool
4. Review `test-results_*.json` and `screenshots/`

### Team Onboarding

1. Clone repository
2. Run `setup.ps1`
3. Read `QUICKSTART.md`
4. Try `run-test.bat https://example.com`
5. Review `README.md` for advanced usage

## File Dependencies

```
TestManiac.CLI.exe
  └─ depends on → TestManiac.Core.dll
                   └─ depends on → Microsoft.Playwright.dll
                                    └─ requires → Playwright browsers

Configuration Files
  └─ used by → CLI Program.cs
                └─ creates → TestConfiguration
                             └─ used by → WebTester

Documentation
  └─ referenced by → README.md (main hub)
```

## Finding Specific Information

| Looking for...       | Check this file                         |
| -------------------- | --------------------------------------- |
| How to install       | `SETUP.md`                              |
| Quick examples       | `QUICKSTART.md`                         |
| All CLI options      | `README.md`                             |
| Architecture details | `PROJECT_OVERVIEW.md`                   |
| Configuration format | `config.example.json`                   |
| Login example        | `config.with-login.example.json`        |
| Core testing logic   | `TestManiac.Core/WebTester.cs`          |
| CLI argument parsing | `TestManiac.CLI/Program.cs`             |
| Event handling       | `TestManiac.Core/WebTester.cs` (events) |
| Result models        | `TestManiac.Core/TestResult.cs`         |

## File Statistics

- **Total Code Files**: 5
- **Documentation Files**: 6
- **Configuration Files**: 2
- **Script Files**: 2
- **Total Lines of Code**: ~1,000+
- **Primary Language**: C# 12

## Getting Started Checklist

- [ ] Read `QUICKSTART.md`
- [ ] Run `setup.ps1` (or follow `SETUP.md`)
- [ ] Copy and edit `config.example.json`
- [ ] Run first test with `--visible` flag
- [ ] Check `test-results_*.json` output
- [ ] Review `screenshots/` folder
- [ ] Read `README.md` for advanced features
- [ ] Explore `PROJECT_OVERVIEW.md` if extending

---

**All files work together to provide a complete automated web testing solution.**
