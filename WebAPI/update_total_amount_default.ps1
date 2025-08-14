# PowerShell script to create a trigger that sets total_amount = total

Write-Host "Creating trigger to automatically set total_amount from total..." -ForegroundColor Green

# Create SQL file with trigger
$sqlContent = @"
-- Create a trigger to automatically set total_amount = total on insert
CREATE TRIGGER IF NOT EXISTS set_total_amount_on_insert
AFTER INSERT ON orders
FOR EACH ROW
WHEN NEW.total_amount IS NULL OR NEW.total_amount = 0
BEGIN
    UPDATE orders 
    SET total_amount = NEW.total 
    WHERE id = NEW.id;
END;

-- Create a trigger to automatically set total_amount = total on update
CREATE TRIGGER IF NOT EXISTS set_total_amount_on_update
AFTER UPDATE ON orders
FOR EACH ROW
WHEN NEW.total_amount IS NULL OR NEW.total_amount = 0
BEGIN
    UPDATE orders 
    SET total_amount = NEW.total 
    WHERE id = NEW.id;
END;
"@

$sqlFile = "temp_create_total_amount_trigger.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Executing SQL to create triggers..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully created triggers for total_amount!" -ForegroundColor Green
    Write-Host "Now total_amount will be automatically set from total column." -ForegroundColor Green
} else {
    Write-Host "There was an error creating the triggers." -ForegroundColor Yellow
}

# Clean up temp file
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue

Write-Host "Done!" -ForegroundColor Green 