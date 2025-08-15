# Azure Deployment Steps

## Prerequisites
- PowerShell installed on your local Windows machine
- Azure CLI installed locally (download from: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows)
- Already logged into Azure CLI (`az login`)

## Step 1: Configure Azure App Service (from Azure Cloud Shell)

If using **Azure Cloud Shell**, run these commands directly:

```bash
# Set variables
RESOURCE_GROUP="cts-vibeappau281"
APP_NAME="cts-vibeappau2812-1"

# Configure app settings
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --settings \
        ASPNETCORE_ENVIRONMENT=Production \
        NPM_CONFIG_PRODUCTION=false \
        WEBSITE_NODE_DEFAULT_VERSION="~18" \
        SCM_DO_BUILD_DURING_DEPLOYMENT=false \
        WEBSITE_RUN_FROM_PACKAGE=0
```

## Step 2: Build and Deploy (from your local machine)

**Open PowerShell on your local Windows machine:**

```powershell
# Navigate to your project directory
cd C:\Users\2325185\code_base\e-commerce-app

# Build Angular app
cd App
npm install
npm run build -- --configuration production

# Copy Angular build to WebAPI wwwroot
cd ..\WebAPI
if (Test-Path "wwwroot") { Remove-Item -Recurse -Force wwwroot }
New-Item -ItemType Directory -Path "wwwroot"
Copy-Item -Path "..\App\dist\e-commerce-app\*" -Destination "wwwroot" -Recurse

# Build and publish .NET API
dotnet restore
dotnet publish -c Release -o ./bin/Release/net8.0/publish

# Create deployment package
cd bin\Release\net8.0\publish
Compress-Archive -Path * -DestinationPath ..\..\..\..\deploy.zip -Force
cd ..\..\..\..

# Deploy to Azure (requires Azure CLI installed locally)
az webapp deployment source config-zip `
    --resource-group cts-vibeappau281 `
    --name cts-vibeappau2812-1 `
    --src deploy.zip
```

## Step 3: Verify Deployment

From Azure Cloud Shell or local machine with Azure CLI:

```bash
# Check deployment
az webapp show --resource-group cts-vibeappau281 --name cts-vibeappau2812-1 --query "state"

# Test the site
curl https://cts-vibeappau2812-1.azurewebsites.net
curl https://cts-vibeappau2812-1.azurewebsites.net/api/products
```

## Alternative: Deploy from VS Code

1. Navigate to: `WebAPI\bin\Release\net8.0\publish`
2. Right-click the `publish` folder
3. Select "Deploy to Web App"
4. Choose your app service

## Troubleshooting

If files aren't appearing in wwwroot:
1. Check Kudu console: https://cts-vibeappau2812-1.scm.azurewebsites.net
2. Navigate to Debug Console → CMD
3. Check: `D:\home\site\wwwroot` 