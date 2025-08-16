# Azure Linux App Service Deployment Instructions

## Option 1: Deploy using tar.gz (Recommended)

1. **Upload the tar.gz file to Azure Cloud Shell**
   - Go to https://shell.azure.com
   - Click the upload button
   - Upload `linux-deploy.tar.gz`

2. **Extract and deploy**
   ```bash
   # Create a clean directory
   mkdir -p ~/deploy-temp
   cd ~/deploy-temp
   
   # Extract the tar file
   tar -xzf ~/linux-deploy.tar.gz
   
   # Create a new zip with proper paths
   zip -r ~/linux-final-deploy.zip *
   
   # Deploy
   az webapp deploy \
       --resource-group cts-vibeappau281 \
       --name cts-vibeappau2812-1 \
       --src-path ~/linux-final-deploy.zip \
       --type zip \
       --restart true
   
   # Clean up
   cd ~
   rm -rf ~/deploy-temp
   ```

## Option 2: Direct deployment from publish folder

Since VS Code deployment worked before, try this approach:

1. **In VS Code**
   - Navigate to `WebAPI\bin\Release\net8.0\publish`
   - Right-click the `publish` folder
   - Select "Deploy to Web App..."
   - Choose `cts-vibeappau2812-1`

## Option 3: Use Kudu ZIP Deploy API directly

1. **Get your deployment credentials**
   ```bash
   az webapp deployment list-publishing-credentials \
       --resource-group cts-vibeappau281 \
       --name cts-vibeappau2812-1 \
       --query "{username:publishingUserName, password:publishingPassword}" \
       --output table
   ```

2. **Use curl to deploy**
   ```bash
   curl -X POST \
       -u <username>:<password> \
       --data-binary @~/linux-final-deploy.zip \
       https://cts-vibeappau2812-1.scm.azurewebsites.net/api/zipdeploy
   ```

## Option 4: Clean deployment with SSH

1. **SSH into your App Service**
   ```bash
   az webapp ssh --resource-group cts-vibeappau281 --name cts-vibeappau2812-1
   ```

2. **Clean the wwwroot**
   ```bash
   cd /home/site/wwwroot
   rm -rf *
   ```

3. **Exit SSH and deploy**
   ```bash
   exit
   # Then use Option 1 or 2
   ```

## Verify Deployment

After deployment, check:
```bash
# Test the site
curl -I https://cts-vibeappau2812-1.azurewebsites.net

# Check logs
az webapp log tail \
    --resource-group cts-vibeappau281 \
    --name cts-vibeappau2812-1
``` 