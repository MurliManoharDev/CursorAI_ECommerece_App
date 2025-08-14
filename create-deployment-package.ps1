# PowerShell script to create deployment package for Azure App Service

Write-Host "Creating deployment package for Azure App Service..." -ForegroundColor Green

# Step 1: Build Angular app for production
Write-Host "`nBuilding Angular app..." -ForegroundColor Yellow
Set-Location -Path "App"
npm install
npm run build:azure

# Step 2: Build and publish .NET API
Write-Host "`nBuilding .NET API..." -ForegroundColor Yellow
Set-Location -Path "../WebAPI"
dotnet restore
dotnet publish -c Release -o ./publish

# Step 3: Create deployment package
Write-Host "`nCreating deployment package..." -ForegroundColor Yellow
Set-Location -Path "publish"

# Remove existing zip if it exists
if (Test-Path "../deploy.zip") {
    Remove-Item "../deploy.zip" -Force
}

# Create zip file
Compress-Archive -Path * -DestinationPath "../deploy.zip" -Force

Set-Location -Path "../.."

Write-Host "`nDeployment package created successfully!" -ForegroundColor Green
Write-Host "Location: WebAPI/deploy.zip" -ForegroundColor Cyan
Write-Host "`nTo deploy to Azure, run:" -ForegroundColor Yellow
Write-Host "az webapp deployment source config-zip --resource-group <your-rg> --name <your-app-name> --src WebAPI/deploy.zip" -ForegroundColor White 