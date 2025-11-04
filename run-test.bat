@echo off
REM TestManiac Quick Run Script
REM Usage: run-test.bat [config-file.json] OR run-test.bat --url <url> [options]

if not "%~1"=="" goto :run_tests

REM No arguments provided, show help
REM No arguments provided, show help
echo ╔══════════════════════════════════════╗
echo ║         TestManiac v1.0              ║
echo ║  Automated Web Testing Tool          ║
echo ╚══════════════════════════════════════╝
echo.
echo Error: Please provide a config file or URL
echo.
echo Usage:
echo   run-test.bat config.json
echo   run-test.bat --url https://example.com [options]
echo.
echo Examples:
echo   run-test.bat myconfig.json
echo   run-test.bat --url https://example.com --visible
echo   run-test.bat --url https://example.com --start-url https://example.com/dashboard --visible
echo   run-test.bat --url https://example.com --username admin --password secret --visible
echo   run-test.bat --url https://example.com --max-pages 20 --max-depth 3 --visible
echo   run-test.bat --url https://example.com --results-path ./results --visible
echo.
echo Available options:
echo   --url ^<url^>                  Base URL to test
echo   --start-url ^<url^>            Start testing from this URL
echo   --username ^<username^>        Login username
echo   --password ^<password^>        Login password
echo   --max-pages ^<number^>         Maximum pages to crawl (default: 50)
echo   --max-depth ^<number^>         Maximum navigation depth (default: 5)
echo   --browser ^<type^>             Browser: chromium, firefox, webkit
echo   --visible                    Run browser in visible mode
echo   --headless ^<true^|false^>     Run in headless mode
echo   --delay ^<ms^>                 Delay between interactions
echo   --timeout ^<ms^>               Navigation timeout
echo   --click-timeout ^<ms^>         Click timeout
echo   --screenshot-path ^<path^>     Screenshots folder
echo   --results-path ^<path^>        Results JSON folder
echo   --no-screenshots             Disable screenshots
echo.
pause
exit /b 1

:run_tests
REM Arguments provided, run tests

echo ╔══════════════════════════════════════╗
echo ║         TestManiac v1.0              ║
echo ║  Automated Web Testing Tool          ║
echo ╚══════════════════════════════════════╝
echo.

REM Check if first argument is a config file (ends with .json)
set "FIRST_ARG=%~1"
echo "%FIRST_ARG%" | findstr /i /r "\.json$" >nul 2>&1
if %ERRORLEVEL% EQU 0 (
    echo Using config file: %FIRST_ARG%
) else (
    echo Running with command-line arguments
)
echo.

dotnet run --project TestManiac.CLI -- %*

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ Test completed successfully
) else (
    echo.
    echo ✗ Test completed with errors - check the results file
)

pause
