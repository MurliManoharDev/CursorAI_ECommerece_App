# Azure App Service Deployment Guide for Angular E-Commerce App

## Overview
This guide provides instructions for deploying the Angular E-Commerce application to Azure App Service with proper production configuration.

## Prerequisites
- Azure subscription
- Azure CLI installed
- Node.js 16.x or higher
- Angular CLI installed globally

## Configuration Changes Made

### 1. Angular Production Configuration
The following optimizations have been added to `angular.json` for production builds:
- File replacements for environment files
- Build optimization enabled
- AOT compilation enabled
- Vendor chunk disabled for smaller bundle size
- Output hashing enabled for cache busting

### 2. Environment Configuration
- `environment.prod.ts` uses relative API paths (`/api`) by default
- Supports absolute URL configuration if API is hosted separately

### 3. Routing Configuration
- Added `web.config` file for proper Angular routing on IIS/Azure
- Handles client-side routing with URL rewriting
- Configured MIME types for static assets

### 4. Build Scripts
Added specialized build scripts in `package.json`:
- `npm run build:prod` - Standard production build
- `npm run build:azure` - Production build with output to WebAPI wwwroot

## Deployment Options

### Option 1: Combined Deployment (Angular + .NET API)
Deploy both Angular and API as a single Azure App Service:

```bash
# Build Angular app
cd App
npm install
npm run build:azure

# Build and publish .NET API
cd ../WebAPI
dotnet restore
dotnet publish -c Release -o ./publish

# Deploy using Azure CLI
az webapp deployment source config-zip \
  --resource-group <your-resource-group> \
  --name <your-app-service-name> \
  --src ./publish.zip
```

### Option 2: Separate Angular Deployment
Deploy Angular app to a separate Azure App Service:

```bash
# Build Angular app
cd App
npm install
npm run build:prod

# Create deployment package
cd dist/e-commerce-app
zip -r ../../angular-app.zip .

# Deploy to Azure
az webapp deployment source config-zip \
  --resource-group <your-resource-group> \
  --name <your-angular-app-service> \
  --src ../../angular-app.zip
```

### Option 3: Using GitHub Actions
The repository includes deployment configuration files (`.deployment` and `deploy.cmd`) for automated deployment via GitHub Actions.

## Azure App Service Configuration

### 1. Application Settings
Configure the following in Azure Portal:
- `WEBSITE_NODE_DEFAULT_VERSION`: Set to `~16` or higher
- `SCM_DO_BUILD_DURING_DEPLOYMENT`: Set to `true` for automated builds

### 2. CORS Settings (if deploying separately)
If Angular and API are on different domains:
- Enable CORS in API App Service
- Add Angular app URL to allowed origins

### 3. HTTPS Configuration
- Enable HTTPS Only in Azure Portal
- Configure SSL certificates if using custom domain

## Troubleshooting

### Common Issues

1. **404 Errors on Refresh**
   - Ensure `web.config` is included in deployment
   - Verify URL rewrite rules are working

2. **API Connection Issues**
   - Check `environment.prod.ts` API URL configuration
   - Verify CORS settings if using separate deployments

3. **Build Failures**
   - Ensure Node.js version matches local development
   - Check npm package compatibility

4. **Large Bundle Size Warning**
   - Consider lazy loading modules
   - Implement code splitting
   - Review and remove unused dependencies

## Performance Optimization

1. **Enable Application Insights**
   - Monitor application performance
   - Track API response times

2. **Configure CDN**
   - Use Azure CDN for static assets
   - Implement proper cache headers

3. **Enable Compression**
   - Gzip compression is enabled by default
   - Verify in browser developer tools

## Security Considerations

1. **Environment Variables**
   - Never commit sensitive data to source control
   - Use Azure Key Vault for secrets

2. **API Security**
   - Implement proper authentication
   - Use HTTPS for all communications

3. **Content Security Policy**
   - Configure CSP headers in web.config
   - Restrict resource loading sources

## Monitoring and Maintenance

1. **Enable Diagnostic Logs**
   ```bash
   az webapp log config \
     --name <app-name> \
     --resource-group <resource-group> \
     --application-logging true \
     --level information
   ```

2. **Set Up Alerts**
   - Configure alerts for HTTP errors
   - Monitor response times

3. **Regular Updates**
   - Keep Angular and dependencies updated
   - Review security advisories

## Additional Resources
- [Azure App Service Documentation](https://docs.microsoft.com/azure/app-service/)
- [Angular Deployment Guide](https://angular.io/guide/deployment)
- [Azure CLI Reference](https://docs.microsoft.com/cli/azure/) 