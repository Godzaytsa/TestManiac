# TestManiac Setup Script
# This script will build the project and install Playwright browsers

Write-Host "╔══════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║      TestManiac Setup Script         ║" -ForegroundColor Cyan
Write-Host "╚══════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Check if .NET is installed
Write-Host "Checking for .NET SDK..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✓ .NET SDK found: $dotnetVersion" -ForegroundColor Green
}
catch {
    Write-Host "✗ .NET SDK not found!" -ForegroundColor Red
    Write-Host "Please install .NET 8.0 SDK from: https://dotnet.microsoft.com/download" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Restore and build the solution
Write-Host "Building TestManiac..." -ForegroundColor Yellow
try {
    dotnet build
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Build successful" -ForegroundColor Green
    }
    else {
        Write-Host "✗ Build failed" -ForegroundColor Red
        exit 1
    }
}
catch {
    Write-Host "✗ Build failed: $_" -ForegroundColor Red
    exit 1
}

Write-Host ""

# Install Playwright browsers
Write-Host "Installing Playwright browsers (this may take a few minutes)..." -ForegroundColor Yellow
try {
    Push-Location TestManiac.CLI\bin\Debug\net8.0
    powershell -File playwright.ps1 install
    Pop-Location
    
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✓ Playwright browsers installed successfully" -ForegroundColor Green
    }
    else {
        Write-Host "⚠ Playwright installation completed with warnings" -ForegroundColor Yellow
    }
}
catch {
    Write-Host "✗ Failed to install Playwright browsers: $_" -ForegroundColor Red
    Pop-Location
    exit 1
}

Write-Host ""
Write-Host "╔══════════════════════════════════════╗" -ForegroundColor Green
Write-Host "║        Setup Complete! 🎉            ║" -ForegroundColor Green
Write-Host "╚══════════════════════════════════════╝" -ForegroundColor Green
Write-Host ""
Write-Host "You can now run TestManiac with:" -ForegroundColor Cyan
Write-Host "  dotnet run --project TestManiac.CLI -- --url https://example.com --visible" -ForegroundColor White
Write-Host ""
Write-Host "For more information, see:" -ForegroundColor Cyan
Write-Host "  - QUICKSTART.md for quick examples" -ForegroundColor White
Write-Host "  - README.md for full documentation" -ForegroundColor White
Write-Host ""
