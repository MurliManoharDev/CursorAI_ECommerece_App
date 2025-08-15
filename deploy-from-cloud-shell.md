# Deploy from Azure Cloud Shell

1. **Open Azure Cloud Shell**: https://shell.azure.com

2. **Upload the deploy.zip file**:
   - Click the "Upload/Download files" button (folder icon) in Cloud Shell toolbar
   - Select "Upload"
   - Browse to `C:\Users\2325185\code_base\e-commerce-app\deploy.zip`
   - Upload the file

3. **Deploy the uploaded file**:
   ```bash
   # The file will be uploaded to your home directory
   ls ~/deploy.zip
   
   # Deploy to your app service
   az webapp deployment source config-zip \
       --resource-group cts-vibeappau281 \
       --name cts-vibeappau2812-1 \
       --src ~/deploy.zip
   ```

4. **Verify deployment**:
   ```bash
   # Check deployment status
   az webapp deployment list \
       --resource-group cts-vibeappau281 \
       --name cts-vibeappau2812-1
   
   # Test the endpoints
   curl https://cts-vibeappau2812-1.azurewebsites.net
   curl https://cts-vibeappau2812-1.azurewebsites.net/api/products
   ``` 