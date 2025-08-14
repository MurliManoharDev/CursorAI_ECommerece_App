# Build script for deploying Angular app and Web API to Azure

Write-Host "Starting build process..." -ForegroundColor Green

# Step 1: Build Angular app for production
Write-Host "`nBuilding Angular app..." -ForegroundColor Yellow
Set-Location -Path "App"
npm install
npm run build -- --configuration production

# Step 2: Create wwwroot directory in WebAPI if it doesn't exist
Write-Host "`nPreparing WebAPI wwwroot directory..." -ForegroundColor Yellow
Set-Location -Path "../WebAPI"
if (!(Test-Path "wwwroot")) {
    New-Item -ItemType Directory -Path "wwwroot" | Out-Null
}

# Step 3: Clear existing wwwroot content
Write-Host "Clearing existing wwwroot content..." -ForegroundColor Yellow
Remove-Item -Path "wwwroot/*" -Recurse -Force -ErrorAction SilentlyContinue

# Step 4: Copy Angular build output to WebAPI wwwroot
Write-Host "Copying Angular build to WebAPI wwwroot..." -ForegroundColor Yellow
Copy-Item -Path "../App/dist/e-commerce-app/*" -Destination "wwwroot" -Recurse -Force

# Step 5: Build and publish Web API
Write-Host "`nBuilding and publishing Web API..." -ForegroundColor Yellow
dotnet restore
dotnet publish -c Release -o ./publish

Write-Host "`nBuild complete!" -ForegroundColor Green
Write-Host "The published output is in: WebAPI/publish" -ForegroundColor Cyan
Write-Host "You can now deploy the contents of the 'publish' folder to Azure App Service." -ForegroundColor Cyan 