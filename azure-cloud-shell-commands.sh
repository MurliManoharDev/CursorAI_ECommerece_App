#!/bin/bash
# Commands to run directly in Azure Cloud Shell

# Variables
RESOURCE_GROUP="cts-vibeappau281"
APP_NAME="cts-vibeappau2812-1"

echo "Configuring Azure App Service..."

# Configure basic settings
az webapp config set \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --net-framework-version "v8.0" \
    --use-32bit-worker-process false

# Configure application settings
az webapp config appsettings set \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --settings \
        ASPNETCORE_ENVIRONMENT=Production \
        NPM_CONFIG_PRODUCTION=false \
        WEBSITE_NODE_DEFAULT_VERSION="~18" \
        SCM_DO_BUILD_DURING_DEPLOYMENT=false \
        WEBSITE_RUN_FROM_PACKAGE=0

# Configure startup command
az webapp config set \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --startup-file "dotnet EcommerceAPI.dll"

# Enable logging
az webapp log config \
    --resource-group $RESOURCE_GROUP \
    --name $APP_NAME \
    --application-logging filesystem \
    --level information \
    --web-server-logging filesystem

echo "Configuration complete!" 