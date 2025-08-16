# Check Azure App Service Logs

## 1. View Real-time Logs
```bash
az webapp log tail \
    --resource-group cts-vibeappau281 \
    --name cts-vibeappau2812-1
```

## 2. Enable Application Logging
```bash
az webapp log config \
    --resource-group cts-vibeappau281 \
    --name cts-vibeappau2812-1 \
    --application-logging filesystem \
    --level information
```

## 3. Check via Kudu
1. Go to: https://cts-vibeappau2812-1.scm.azurewebsites.net
2. Navigate to **Debug Console** → **Bash**
3. Check for database file:
   ```bash
   ls -la /home/
   ls -la /home/site/wwwroot/
   find /home -name "*.db" 2>/dev/null
   ```

## 4. Test Database Connection
Add this temporary endpoint to test database:

```csharp
[HttpGet("test-db")]
public IActionResult TestDatabase()
{
    try
    {
        var dbPath = _context.Database.GetConnectionString();
        var exists = System.IO.File.Exists("/home/ecommerce.db");
        var categories = _context.Categories.Count();
        
        return Ok(new 
        { 
            connectionString = dbPath,
            databaseExists = exists,
            categoriesCount = categories,
            canWrite = IsDirectoryWritable("/home")
        });
    }
    catch (Exception ex)
    {
        return Ok(new { error = ex.Message, stackTrace = ex.StackTrace });
    }
}

private bool IsDirectoryWritable(string path)
{
    try
    {
        var testFile = Path.Combine(path, "test.txt");
        System.IO.File.WriteAllText(testFile, "test");
        System.IO.File.Delete(testFile);
        return true;
    }
    catch
    {
        return false;
    }
}
``` 