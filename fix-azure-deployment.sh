#!/bin/bash
# Fix Azure deployment settings

# Variables
RG="cts-vibeappau281"
APP="cts-vibeappau2812-1"

echo "Configuring Azure App Service to use pre-built files..."

# Step 1: Disable Oryx build
az webapp config appsettings set \
    --resource-group $RG \
    --name $APP \
    --settings \
        SCM_DO_BUILD_DURING_DEPLOYMENT=false \
        ENABLE_ORYX_BUILD=false \
        SCM_SCRIPT_GENERATOR_ARGS="--no-build"

# Step 2: Set the correct startup command
az webapp config set \
    --resource-group $RG \
    --name $APP \
    --startup-file "dotnet EcommerceAPI.dll"

# Step 3: Restart the app
echo "Restarting app service..."
az webapp restart --resource-group $RG --name $APP

echo "Configuration complete!"
echo ""
echo "Now redeploy your zip file:"
echo "az webapp deployment source config-zip --resource-group $RG --name $APP --src ~/deploy-complete.zip" 