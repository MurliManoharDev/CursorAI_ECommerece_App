# PowerShell script to properly deploy to Azure App Service

Write-Host "Starting Azure deployment..." -ForegroundColor Green

# Variables
$resourceGroup = "cts-vibeappau281"
$appServiceName = "cts-vibeappau2812-1"

# Step 1: Build and prepare deployment
Write-Host "`nBuilding application..." -ForegroundColor Yellow

# Build Angular app
Set-Location -Path "App"
npm install
npm run build:azure

# Build .NET API
Set-Location -Path "../WebAPI"
dotnet restore
dotnet publish -c Release -o ./bin/Release/net8.0/publish

# Step 2: Create deployment package from the publish folder
Write-Host "`nCreating deployment package..." -ForegroundColor Yellow
Set-Location -Path "bin/Release/net8.0/publish"

# Remove existing zip if it exists
if (Test-Path "../../../../deploy.zip") {
    Remove-Item "../../../../deploy.zip" -Force
}

# Create zip file from the publish directory contents
Compress-Archive -Path * -DestinationPath "../../../../deploy.zip" -Force

Set-Location -Path "../../../.."

# Step 3: Deploy to Azure
Write-Host "`nDeploying to Azure App Service..." -ForegroundColor Yellow

# Check if Azure CLI is installed
$azureCliInstalled = Get-Command az -ErrorAction SilentlyContinue
if ($azureCliInstalled) {
    # Deploy using Azure CLI
    az webapp deployment source config-zip `
        --resource-group $resourceGroup `
        --name $appServiceName `
        --src deploy.zip
} else {
    Write-Host "Azure CLI not found. Please install Azure CLI or use the Azure Portal to deploy deploy.zip" -ForegroundColor Red
    Write-Host "Download Azure CLI from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli" -ForegroundColor Yellow
}

Write-Host "`nDeployment complete!" -ForegroundColor Green 