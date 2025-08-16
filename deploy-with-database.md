# Deploy with Development Database

Your `linux-deploy-with-db.tar.gz` package now includes:
- All application files
- Your development database (`ecommerce.db`) with all existing data
- A startup script (`init-db.sh`) to ensure the database is in the correct location

## Deployment Steps

1. **Upload to Azure Cloud Shell**
   - Go to https://shell.azure.com
   - Click the upload button
   - Upload `linux-deploy-with-db.tar.gz`

2. **Deploy the package**
   ```bash
   # Create clean directory
   mkdir -p ~/deploy-temp && cd ~/deploy-temp
   
   # Extract
   tar -xzf ~/linux-deploy-with-db.tar.gz
   
   # Run the database initialization script
   chmod +x init-db.sh
   ./init-db.sh
   
   # Create deployment zip
   zip -r ~/final-deploy-with-db.zip *
   
   # Deploy
   az webapp deploy \
       --resource-group cts-vibeappau281 \
       --name cts-vibeappau2812-1 \
       --src-path ~/final-deploy-with-db.zip \
       --type zip \
       --restart true
   
   # Clean up
   cd ~
   rm -rf ~/deploy-temp
   ```

3. **After deployment, ensure database is copied**
   ```bash
   # SSH into the app
   az webapp ssh --resource-group cts-vibeappau281 --name cts-vibeappau2812-1
   
   # Once in SSH, check and copy database if needed
   if [ ! -f "/home/ecommerce.db" ] && [ -f "/home/site/wwwroot/ecommerce.db" ]; then
       cp /home/site/wwwroot/ecommerce.db /home/ecommerce.db
       chmod 666 /home/ecommerce.db
       echo "Database copied successfully"
   fi
   
   # Check database
   ls -la /home/ecommerce.db
   
   # Exit SSH
   exit
   ```

4. **Verify deployment**
   - Test database endpoint: https://cts-vibeappau2812-1.azurewebsites.net/api/categories/db-test
   - Check categories: https://cts-vibeappau2812-1.azurewebsites.net/api/categories
   - Check products: https://cts-vibeappau2812-1.azurewebsites.net/api/products

## Alternative: Direct Copy via Kudu

1. Go to: https://cts-vibeappau2812-1.scm.azurewebsites.net
2. Navigate to **Debug Console** → **Bash**
3. Upload your `ecommerce.db` file directly
4. Run:
   ```bash
   cp /home/site/wwwroot/ecommerce.db /home/ecommerce.db
   chmod 666 /home/ecommerce.db
   ```

This approach uses your existing development database with all its data, avoiding the need for seed data! 