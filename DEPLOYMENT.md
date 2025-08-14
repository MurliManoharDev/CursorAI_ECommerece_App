# Azure Deployment Instructions

This guide explains how to deploy both the Angular frontend and .NET Web API to a single Azure App Service at `https://cts-vibeappau2812-1.azurewebsites.net`.

## Deployment Architecture

- **Frontend (Angular)**: Served from root path `/`
- **Backend (Web API)**: Served from `/api/*` path
- **Domain**: `https://cts-vibeappau2812-1.azurewebsites.net`

## Prerequisites

1. Azure App Service created and configured
2. Node.js and npm installed locally
3. .NET 8 SDK installed locally
4. PowerShell (for running build script)

## Build and Deploy Steps

### Option 1: Using the Build Script (Recommended)

1. Open PowerShell in the project root directory
2. Run the build script:
   ```powershell
   .\build-and-deploy.ps1
   ```
3. The script will:
   - Build the Angular app for production
   - Copy the Angular build to WebAPI/wwwroot
   - Build and publish the Web API
   - Output will be in `WebAPI/publish` folder

### Option 2: Manual Build Steps

1. **Build Angular App**
   ```bash
   cd App
   npm install
   npm run build -- --configuration production
   ```

2. **Prepare Web API**
   ```bash
   cd ../WebAPI
   mkdir wwwroot (if not exists)
   ```

3. **Copy Angular build to wwwroot**
   - Copy all files from `App/dist/e-commerce-app/*` to `WebAPI/wwwroot/`

4. **Build Web API**
   ```bash
   dotnet restore
   dotnet publish -c Release -o ./publish
   ```

### Deploy to Azure

1. **Using Azure CLI**
   ```bash
   az webapp deployment source config-zip --resource-group <your-rg> --name cts-vibeappau2812-1 --src WebAPI/publish.zip
   ```

2. **Using Visual Studio**
   - Right-click on the EcommerceAPI project
   - Select "Publish"
   - Choose "Azure App Service" and follow the wizard

3. **Using FTP/FTPS**
   - Get FTP credentials from Azure Portal
   - Upload contents of `WebAPI/publish` folder to `/site/wwwroot`

## Azure App Service Configuration

### Application Settings

Add these application settings in Azure Portal:

```
ASPNETCORE_ENVIRONMENT=Production
```

### Connection Strings

If using Azure SQL instead of SQLite, update the connection string in Azure Portal.

## Verify Deployment

1. **Frontend**: Navigate to `https://cts-vibeappau2812-1.azurewebsites.net`
2. **API Health Check**: Navigate to `https://cts-vibeappau2812-1.azurewebsites.net/api/products`
3. **Swagger (if enabled)**: `https://cts-vibeappau2812-1.azurewebsites.net/api/swagger`

## Troubleshooting

### Common Issues

1. **404 Errors for Angular Routes**
   - Ensure web.config is properly deployed
   - Check URL rewrite rules

2. **API Not Accessible**
   - Verify UsePathBase("/api") is configured in Program.cs
   - Check CORS configuration

3. **Static Files Not Loading**
   - Ensure wwwroot folder is included in publish
   - Check mime types in web.config

### Enable Logging

To enable detailed logging in Azure:

1. Go to Azure Portal > App Service > App Service logs
2. Enable Application Logging (Filesystem)
3. Set Level to "Information" or "Verbose"
4. View logs in Log Stream

## Security Considerations

Before going to production:

1. **Update JWT Secret Key** in appsettings.Production.json
2. **Configure HTTPS** only in Azure App Service
3. **Update CORS origins** to match your production domain
4. **Secure database** connection strings
5. **Update Stripe keys** to production keys

## Maintenance

### Update Deployment

1. Make code changes
2. Run build script again
3. Deploy updated `publish` folder

### Database Migrations

If using Entity Framework migrations:
```bash
dotnet ef database update --connection "<Azure SQL Connection String>"
``` 