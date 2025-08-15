# PowerShell script to verify Azure deployment

Write-Host "Verifying Azure deployment..." -ForegroundColor Green

# Variables
$resourceGroup = "cts-vibeappau281"
$appServiceName = "cts-vibeappau2812-1"
$appUrl = "https://cts-vibeappau2812-1.azurewebsites.net"

# Step 1: Check app service status
Write-Host "`nChecking App Service status..." -ForegroundColor Yellow
az webapp show `
    --resource-group $resourceGroup `
    --name $appServiceName `
    --query "state" `
    --output tsv

# Step 2: List files in wwwroot (using Kudu API)
Write-Host "`nListing files in wwwroot via Kudu..." -ForegroundColor Yellow
$kuduUrl = "https://$appServiceName.scm.azurewebsites.net/api/vfs/site/wwwroot/"
Write-Host "Kudu URL: $kuduUrl"
Write-Host "Note: You'll need to authenticate with your Azure credentials"

# Step 3: Test endpoints
Write-Host "`nTesting application endpoints..." -ForegroundColor Yellow

# Test home page
Write-Host "Testing home page..."
try {
    $response = Invoke-WebRequest -Uri $appUrl -UseBasicParsing
    Write-Host "Home page status: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "Home page error: $_" -ForegroundColor Red
}

# Test API endpoint
Write-Host "`nTesting API endpoint..."
try {
    $response = Invoke-WebRequest -Uri "$appUrl/api/products" -UseBasicParsing
    Write-Host "API status: $($response.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "API error: $_" -ForegroundColor Red
}

# Step 4: Get recent logs
Write-Host "`nGetting recent application logs..." -ForegroundColor Yellow
az webapp log tail `
    --resource-group $resourceGroup `
    --name $appServiceName `
    --timeout 30

Write-Host "`nVerification complete!" -ForegroundColor Green 