# Manual Deployment via Kudu (Most Reliable Method)

## Step 1: Access Kudu Console
1. Open your browser and go to: https://cts-vibeappau2812-1.scm.azurewebsites.net
2. Login with your Azure credentials
3. Click on **"Debug console"** → **"CMD"**

## Step 2: Clean the wwwroot
In the console, run these commands:
```cmd
cd D:\home\site\wwwroot
del /Q /S *.*
for /d %i in (*) do rd /s /q "%i"
dir
```

## Step 3: Upload and Extract Your Zip
1. Stay in the CMD console at `D:\home\site\wwwroot`
2. Drag and drop your `final-deploy.zip` file into the file browser area above the console
3. Once uploaded, run:
```cmd
unzip final-deploy.zip
del final-deploy.zip
dir
```

## Step 4: Verify Files
You should see:
- `EcommerceAPI.dll` and other DLL files
- `index.html`
- `main.*.js`, `polyfills.*.js`, `runtime.*.js`
- `styles.*.css`
- `assets` folder
- `web.config`

## Step 5: Restart the App Service
In Azure Portal or CLI:
```bash
az webapp restart --resource-group cts-vibeappau281 --name cts-vibeappau2812-1
```

## Alternative: Using PowerShell in Kudu
If unzip doesn't work, use PowerShell console in Kudu:
```powershell
cd D:\home\site\wwwroot
Expand-Archive -Path final-deploy.zip -DestinationPath . -Force
Remove-Item final-deploy.zip
Get-ChildItem
```

## Why This Works
- Bypasses all deployment scripts and Oryx build system
- Places files exactly where they need to be
- No transformation or build process to interfere
- Direct control over file placement 