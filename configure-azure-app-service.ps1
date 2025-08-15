# PowerShell script to configure Azure App Service for proper deployment

Write-Host "Configuring Azure App Service..." -ForegroundColor Green

# Variables
$resourceGroup = "cts-vibeappau281"
$appServiceName = "cts-vibeappau2812-1"

# Step 1: Configure App Service basic settings
Write-Host "`nConfiguring basic settings..." -ForegroundColor Yellow

# Set .NET 8 runtime
az webapp config set `
    --resource-group $resourceGroup `
    --name $appServiceName `
    --net-framework-version "v8.0" `
    --use-32bit-worker-process false

# Step 2: Configure application settings
Write-Host "`nConfiguring application settings..." -ForegroundColor Yellow

# Set environment variables
az webapp config appsettings set `
    --resource-group $resourceGroup `
    --name $appServiceName `
    --settings `
        ASPNETCORE_ENVIRONMENT=Production `
        NPM_CONFIG_PRODUCTION=false `
        WEBSITE_NODE_DEFAULT_VERSION="~18" `
        SCM_DO_BUILD_DURING_DEPLOYMENT=false `
        WEBSITE_RUN_FROM_PACKAGE=0

# Step 3: Configure deployment settings
Write-Host "`nConfiguring deployment settings..." -ForegroundColor Yellow

# Set deployment source to local git (if using Git deployment)
# az webapp deployment source config-local-git `
#     --resource-group $resourceGroup `
#     --name $appServiceName

# Step 4: Configure startup command (if needed)
Write-Host "`nConfiguring startup command..." -ForegroundColor Yellow

az webapp config set `
    --resource-group $resourceGroup `
    --name $appServiceName `
    --startup-file "dotnet EcommerceAPI.dll"

# Step 5: Enable logging for troubleshooting
Write-Host "`nEnabling application logging..." -ForegroundColor Yellow

az webapp log config `
    --resource-group $resourceGroup `
    --name $appServiceName `
    --application-logging filesystem `
    --level information `
    --web-server-logging filesystem

# Step 6: Set the physical path for the site
Write-Host "`nConfiguring virtual applications..." -ForegroundColor Yellow

# This ensures the app looks in the right place for files
az webapp config set `
    --resource-group $resourceGroup `
    --name $appServiceName `
    --linux-fx-version "DOTNETCORE|8.0"

Write-Host "`nConfiguration complete!" -ForegroundColor Green
Write-Host "You can now deploy your application using deploy-to-azure.ps1" -ForegroundColor Cyan 