# Check deployment status and test endpoints

Write-Host "Checking deployment status..." -ForegroundColor Green

$appUrl = "https://cts-vibeappau2812-1.azurewebsites.net"

# Test 1: Check if site is responding
Write-Host "`nTesting main site..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri $appUrl -UseBasicParsing -TimeoutSec 30
    Write-Host "Site Status: $($response.StatusCode) - $($response.StatusDescription)" -ForegroundColor Green
    
    # Check if we got HTML content
    if ($response.Content -like "*<!DOCTYPE html>*" -or $response.Content -like "*<html*") {
        Write-Host "✓ HTML content detected - Angular app is responding" -ForegroundColor Green
    } else {
        Write-Host "⚠ No HTML content detected" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ Site not responding: $_" -ForegroundColor Red
}

# Test 2: Check API endpoint
Write-Host "`nTesting API endpoint..." -ForegroundColor Yellow
try {
    $apiResponse = Invoke-WebRequest -Uri "$appUrl/api/products" -UseBasicParsing -TimeoutSec 30
    Write-Host "API Status: $($apiResponse.StatusCode)" -ForegroundColor Green
    
    # Try to parse JSON
    try {
        $json = $apiResponse.Content | ConvertFrom-Json
        Write-Host "✓ API is returning valid JSON" -ForegroundColor Green
    } catch {
        Write-Host "⚠ API response is not valid JSON" -ForegroundColor Yellow
    }
} catch {
    Write-Host "✗ API not responding: $_" -ForegroundColor Red
}

# Test 3: Check specific files
Write-Host "`nChecking for key files..." -ForegroundColor Yellow
$filesToCheck = @(
    "/index.html",
    "/main.6162ce5520dfd2e0.js",
    "/styles.8e94b24b7162583a.css",
    "/favicon.ico"
)

foreach ($file in $filesToCheck) {
    try {
        $fileResponse = Invoke-WebRequest -Uri "$appUrl$file" -Method Head -UseBasicParsing -TimeoutSec 10
        Write-Host "✓ $file - Found" -ForegroundColor Green
    } catch {
        Write-Host "✗ $file - Not Found" -ForegroundColor Red
    }
}

Write-Host "`nDeployment check complete!" -ForegroundColor Green
Write-Host "`nIf files are missing, the deployment might have succeeded but files weren't extracted properly." -ForegroundColor Yellow
Write-Host "Visit the site directly: $appUrl" -ForegroundColor Cyan 