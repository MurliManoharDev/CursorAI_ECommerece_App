# Deploy using Run From Package method - More reliable for Azure App Service

Write-Host "Deploying to Azure using Run From Package..." -ForegroundColor Green

# Variables
$resourceGroup = "cts-vibeappau281"
$appServiceName = "cts-vibeappau2812-1"
$zipPath = "C:\Users\2325185\code_base\e-commerce-app\final-deploy.zip"

# Step 1: Upload the zip to Azure Storage (built-in)
Write-Host "`nUploading deployment package..." -ForegroundColor Yellow

# Configure app to run from package
az webapp config appsettings set `
    --resource-group $resourceGroup `
    --name $appServiceName `
    --settings WEBSITE_RUN_FROM_PACKAGE="1"

# Step 2: Deploy using zipdeploy with run-from-package
Write-Host "`nDeploying package..." -ForegroundColor Yellow

az webapp deployment source config-zip `
    --resource-group $resourceGroup `
    --name $appServiceName `
    --src $zipPath `
    --timeout 600

# Step 3: Restart the app
Write-Host "`nRestarting app service..." -ForegroundColor Yellow

az webapp restart `
    --resource-group $resourceGroup `
    --name $appServiceName

Write-Host "`nDeployment complete!" -ForegroundColor Green
Write-Host "Check your app at: https://cts-vibeappau2812-1.azurewebsites.net" -ForegroundColor Cyan 