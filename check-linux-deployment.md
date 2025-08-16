# Linux App Service Deployment Guide

## Your App Service is Running on Linux!

The Docker logs indicate you have a **Linux-based App Service**, which requires different deployment approaches.

## Option 1: Deploy via Azure Portal (Easiest)
1. Go to Azure Portal: https://portal.azure.com
2. Navigate to your App Service: `cts-vibeappau2812-1`
3. Go to **Deployment Center** (in the left menu)
4. Choose **Local Git** or **GitHub Actions**
5. Or use **Advanced Tools (Kudu)** → **Bash** console

## Option 2: Use VS Code Deployment
The VS Code Azure extension works with Linux App Services:
1. Right-click on `publish` folder in VS Code
2. Select "Deploy to Web App..."
3. It will handle Linux deployment automatically

## Option 3: Azure CLI Commands for Linux
From Azure Cloud Shell:

```bash
# Check current runtime
az webapp config show --resource-group cts-vibeappau281 --name cts-vibeappau2812-1 --query linuxFxVersion

# Set .NET 8 runtime for Linux
az webapp config set --resource-group cts-vibeappau281 --name cts-vibeappau2812-1 --linux-fx-version "DOTNETCORE|8.0"

# Deploy your zip
az webapp deploy --resource-group cts-vibeappau281 --name cts-vibeappau2812-1 --src-path ~/final-deploy.zip --type zip
```

## Option 4: Direct File Access via Kudu SSH
1. Go to: https://cts-vibeappau2812-1.scm.azurewebsites.net/webssh/host
2. This gives you SSH access to the Linux container
3. Navigate to: `/home/site/wwwroot`
4. Upload and extract files there

## Why Your Deployment Might Be Failing
1. **Wrong paths**: Linux uses `/home/site/wwwroot`, not `D:\home\site\wwwroot`
2. **Oryx build system**: Linux App Services use Oryx which might be interfering
3. **Container configuration**: The .NET runtime might not be properly configured

## Quick Fix via Azure Cloud Shell
```bash
# Disable Oryx build
az webapp config appsettings set --resource-group cts-vibeappau281 --name cts-vibeappau2812-1 --settings ENABLE_ORYX_BUILD=false SCM_DO_BUILD_DURING_DEPLOYMENT=false

# Set startup command
az webapp config set --resource-group cts-vibeappau281 --name cts-vibeappau2812-1 --startup-file "dotnet EcommerceAPI.dll"

# Deploy
az webapp deploy --resource-group cts-vibeappau281 --name cts-vibeappau2812-1 --src-path ~/final-deploy.zip --type zip --restart true
``` 