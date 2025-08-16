# Diagnose Application Error

## 1. Check Application Logs
```bash
az webapp log tail --resource-group cts-vibeappau281 --name cts-vibeappau2812-1
```

## 2. Check Recent Deployment Logs
```bash
az webapp log deployment show --resource-group cts-vibeappau281 --name cts-vibeappau2812-1 --deployment-id latest
```

## 3. Enable Detailed Error Messages
```bash
az webapp config appsettings set --resource-group cts-vibeappau281 --name cts-vibeappau2812-1 --settings ASPNETCORE_ENVIRONMENT=Development ASPNETCORE_DETAILEDERRORS=true
```

## 4. Check via Kudu
Go to: https://cts-vibeappau2812-1.scm.azurewebsites.net
- Navigate to **Debug Console** → **Bash**
- Check the logs:
```bash
cd /home/LogFiles
ls -la
cat [latest log file]
```

## 5. SSH and Check Directly
```bash
az webapp ssh --resource-group cts-vibeappau281 --name cts-vibeappau2812-1
```

Once in SSH:
```bash
# Check if app is running
ps aux | grep dotnet

# Check for startup errors
cd /home/site/wwwroot
dotnet EcommerceAPI.dll

# Check database
ls -la /home/ecommerce.db
ls -la /home/site/wwwroot/ecommerce.db
``` 