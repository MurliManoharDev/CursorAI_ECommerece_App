# Diagnostic script to check deployment readiness

Write-Host "Checking deployment configuration..." -ForegroundColor Cyan

# Check if wwwroot exists
Write-Host "`nChecking wwwroot directory..." -ForegroundColor Yellow
if (Test-Path "WebAPI/wwwroot") {
    Write-Host "✓ wwwroot exists" -ForegroundColor Green
    
    # Check if index.html exists
    if (Test-Path "WebAPI/wwwroot/index.html") {
        Write-Host "✓ index.html found" -ForegroundColor Green
    } else {
        Write-Host "✗ index.html NOT found - Angular build missing!" -ForegroundColor Red
        Write-Host "  Run: .\build-and-deploy.ps1 to build Angular app" -ForegroundColor Yellow
    }
    
    # List wwwroot contents
    Write-Host "`nContents of wwwroot:" -ForegroundColor Cyan
    Get-ChildItem "WebAPI/wwwroot" | Select-Object Name, Length, LastWriteTime | Format-Table
} else {
    Write-Host "✗ wwwroot directory NOT found!" -ForegroundColor Red
    Write-Host "  Run: .\build-and-deploy.ps1 to create and populate it" -ForegroundColor Yellow
}

# Check web.config
Write-Host "`nChecking web.config..." -ForegroundColor Yellow
if (Test-Path "WebAPI/web.config") {
    Write-Host "✓ web.config exists" -ForegroundColor Green
} else {
    Write-Host "✗ web.config NOT found!" -ForegroundColor Red
}

Write-Host "`nNext steps:" -ForegroundColor Cyan
Write-Host "1. If Angular files are missing, run: .\build-and-deploy.ps1" -ForegroundColor White
Write-Host "2. Deploy the 'WebAPI/publish' folder to Azure" -ForegroundColor White
Write-Host "3. Access your app at: https://cts-vibeappau2812-1.azurewebsites.net/" -ForegroundColor White 