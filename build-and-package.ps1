# PowerShell script to build and package the application correctly
Write-Host "Building E-commerce Application..." -ForegroundColor Green

# Step 1: Build Angular App
Write-Host "`nBuilding Angular application..." -ForegroundColor Yellow
Set-Location "C:\Users\2325185\code_base\e-commerce-app\App"
npm install
npm run build -- --configuration production

# Step 2: Clean WebAPI wwwroot (to avoid nested structure)
Write-Host "`nCleaning WebAPI wwwroot..." -ForegroundColor Yellow
Set-Location "C:\Users\2325185\code_base\e-commerce-app\WebAPI"
if (Test-Path "wwwroot") {
    Remove-Item -Recurse -Force wwwroot
}

# Step 3: Publish .NET API (without wwwroot)
Write-Host "`nPublishing .NET API..." -ForegroundColor Yellow
dotnet publish -c Release -o ./bin/Release/net8.0/publish

# Step 4: Copy Angular files directly to publish folder (not in wwwroot subfolder)
Write-Host "`nCopying Angular files to publish folder..." -ForegroundColor Yellow
$sourceAngular = "C:\Users\2325185\code_base\e-commerce-app\App\dist\e-commerce-app"
$destPublish = "C:\Users\2325185\code_base\e-commerce-app\WebAPI\bin\Release\net8.0\publish"

# Copy Angular files to root of publish folder
Copy-Item -Path "$sourceAngular\*" -Destination $destPublish -Recurse -Force

# Step 5: Copy database to publish folder
Write-Host "`nCopying database..." -ForegroundColor Yellow
Copy-Item "C:\Users\2325185\code_base\e-commerce-app\WebAPI\ecommerce.db" -Destination "$destPublish\ecommerce.db" -Force

# Copy init-db.sh script
Copy-Item "C:\Users\2325185\code_base\e-commerce-app\WebAPI\init-db.sh" -Destination "$destPublish\init-db.sh" -Force

# Step 6: Create deployment package
Write-Host "`nCreating deployment package..." -ForegroundColor Yellow
Set-Location $destPublish

# Create tar.gz for Linux deployment
tar -czf "C:\Users\2325185\code_base\e-commerce-app\deploy-final.tar.gz" *

Set-Location "C:\Users\2325185\code_base\e-commerce-app"
Write-Host "`nBuild complete!" -ForegroundColor Green
Write-Host "Deployment package created: deploy-final.tar.gz" -ForegroundColor Cyan
Write-Host "`nFile structure in package:" -ForegroundColor Yellow
tar -tzf deploy-final.tar.gz | Select-Object -First 20 