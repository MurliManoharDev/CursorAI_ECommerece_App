# Final Deployment Instructions - Correctly Structured Package

Your `deploy-final.tar.gz` (26.99 MB) contains:
- ✅ Angular files at root level (index.html, *.js, *.css)
- ✅ .NET API files (EcommerceAPI.dll, etc.)
- ✅ Database (ecommerce.db)
- ✅ Database init script (init-db.sh)
- ✅ All assets properly structured

## Deployment Steps

1. **Upload to Azure Cloud Shell**
   - Go to https://shell.azure.com
   - Click the upload button (folder icon)
   - Upload `deploy-final.tar.gz`

2. **Deploy the package**
   ```bash
   # Extract and prepare
   mkdir -p ~/deploy-temp && cd ~/deploy-temp
   tar -xzf ~/deploy-final.tar.gz
   
   # Run database init script
   chmod +x init-db.sh
   ./init-db.sh
   
   # Create deployment zip
   zip -r ~/final-deploy.zip *
   
   # Deploy
   az webapp deploy --resource-group cts-vibeappau281 --name cts-vibeappau2812-1 --src-path ~/final-deploy.zip --type zip --restart true
   
   # Clean up
   cd ~ && rm -rf ~/deploy-temp
   ```

3. **Verify deployment**
   - Main site: https://cts-vibeappau2812-1.azurewebsites.net
   - API test: https://cts-vibeappau2812-1.azurewebsites.net/api/categories
   - Database test: https://cts-vibeappau2812-1.azurewebsites.net/api/categories/db-test

## Post-Deployment Database Setup

If the database needs to be moved to /home:
```bash
az webapp ssh --resource-group cts-vibeappau281 --name cts-vibeappau2812-1
```

Once in SSH:
```bash
# Check if database is in correct location
if [ ! -f "/home/ecommerce.db" ] && [ -f "/home/site/wwwroot/ecommerce.db" ]; then
    cp /home/site/wwwroot/ecommerce.db /home/ecommerce.db
    chmod 666 /home/ecommerce.db
    echo "Database moved to /home"
fi

# Verify
ls -la /home/ecommerce.db
exit
```

## This Package Fixes

- ✅ No nested wwwroot directories
- ✅ Angular files at correct level
- ✅ Database included with init script
- ✅ Proper file structure for Azure Linux App Service

The app should work immediately after deployment! 