@echo off
REM TestManiac Quick Run Script
REM Usage: run-test.bat https://example.com

echo ╔══════════════════════════════════════╗
echo ║         TestManiac v1.0              ║
echo ║  Automated Web Testing Tool          ║
echo ╚══════════════════════════════════════╝
echo.

if "%1"=="" (
    echo Error: Please provide a URL
    echo Usage: run-test.bat https://example.com
    echo.
    echo Optional parameters:
    echo   run-test.bat https://example.com visible
    echo   run-test.bat https://example.com headless
    exit /b 1
)

set URL=%1
set MODE=--visible

if "%2"=="headless" (
    set MODE=--headless true
)

echo Testing URL: %URL%
echo Mode: %MODE%
echo.

dotnet run --project TestManiac.CLI -- --url %URL% %MODE% --max-pages 20 --max-depth 3

if %ERRORLEVEL% EQU 0 (
    echo.
    echo ✓ Test completed successfully
) else (
    echo.
    echo ✗ Test completed with errors - check the results file
)

pause
