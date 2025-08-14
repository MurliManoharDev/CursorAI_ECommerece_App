# PowerShell script to make user_id column nullable in orders table

Write-Host "Fixing user_id column to allow NULL values (for guest checkout)..." -ForegroundColor Green

# In SQLite, we can't directly ALTER COLUMN to change constraints
# We need to recreate the table or use a workaround
$sqlContent = @"
-- Create a new orders table with user_id as nullable
CREATE TABLE orders_new (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    order_number TEXT NOT NULL,
    user_id INTEGER,  -- This is now nullable
    first_name TEXT,
    last_name TEXT,
    company_name TEXT,
    email TEXT,
    phone_number TEXT,
    street_address TEXT,
    apartment_suite TEXT,
    city TEXT,
    state TEXT,
    country TEXT,
    zip_code TEXT,
    subtotal REAL DEFAULT 0,
    shipping_cost REAL DEFAULT 0,
    tax REAL DEFAULT 0,
    total REAL DEFAULT 0,
    order_notes TEXT,
    payment_method TEXT,
    payment_intent_id TEXT,
    stripe_payment_id TEXT,
    payment_status INTEGER DEFAULT 0,
    status INTEGER DEFAULT 0,
    created_at TEXT,
    updated_at TEXT,
    shipped_at TEXT,
    delivered_at TEXT,
    FOREIGN KEY (user_id) REFERENCES users(id)
);

-- Copy existing data from orders to orders_new
INSERT INTO orders_new SELECT * FROM orders;

-- Drop the old orders table
DROP TABLE orders;

-- Rename orders_new to orders
ALTER TABLE orders_new RENAME TO orders;
"@

$sqlFile = "temp_fix_user_id_nullable.sql"

# Write SQL to file
$sqlContent | Out-File -FilePath $sqlFile -Encoding UTF8

# Execute SQL file
Write-Host "Executing SQL to fix user_id column..."
& dotnet run --project ../SeedDatabase/ExecuteSqlFile.csproj $sqlFile ../WebAPI/ecommerce.db

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Successfully fixed user_id column to allow NULL values!" -ForegroundColor Green
    Write-Host "Guest checkout (without user login) is now supported." -ForegroundColor Green
} else {
    Write-Host "There was an error fixing the column." -ForegroundColor Red
}

# Clean up temp file
Remove-Item $sqlFile -Force -ErrorAction SilentlyContinue

Write-Host "Done!" -ForegroundColor Green 