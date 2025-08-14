-- Add missing columns to orders table
-- These are the columns that are causing errors based on the payload

-- Address fields that are missing
ALTER TABLE orders ADD COLUMN city TEXT;
ALTER TABLE orders ADD COLUMN state TEXT;
ALTER TABLE orders ADD COLUMN country TEXT;
ALTER TABLE orders ADD COLUMN zip_code TEXT;

-- Billing information fields that might be missing
ALTER TABLE orders ADD COLUMN first_name TEXT;
ALTER TABLE orders ADD COLUMN last_name TEXT;
ALTER TABLE orders ADD COLUMN company_name TEXT;
ALTER TABLE orders ADD COLUMN email TEXT;
ALTER TABLE orders ADD COLUMN phone_number TEXT;
ALTER TABLE orders ADD COLUMN street_address TEXT;

-- Order details that might be missing
ALTER TABLE orders ADD COLUMN order_notes TEXT;

-- Payment information that might be missing
ALTER TABLE orders ADD COLUMN payment_method TEXT;
ALTER TABLE orders ADD COLUMN payment_intent_id TEXT; 