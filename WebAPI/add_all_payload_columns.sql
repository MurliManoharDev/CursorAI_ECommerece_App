-- Add all columns needed based on the actual payload
-- Using snake_case naming convention as per DbContext configuration

-- Billing/Contact Information
ALTER TABLE orders ADD COLUMN first_name TEXT;
ALTER TABLE orders ADD COLUMN last_name TEXT;
ALTER TABLE orders ADD COLUMN company_name TEXT;
ALTER TABLE orders ADD COLUMN email TEXT;
ALTER TABLE orders ADD COLUMN phone_number TEXT;

-- Address Information
ALTER TABLE orders ADD COLUMN street_address TEXT;
ALTER TABLE orders ADD COLUMN apartment_suite TEXT;
ALTER TABLE orders ADD COLUMN city TEXT;
ALTER TABLE orders ADD COLUMN state TEXT;
ALTER TABLE orders ADD COLUMN country TEXT;
ALTER TABLE orders ADD COLUMN zip_code TEXT;

-- Order Information
ALTER TABLE orders ADD COLUMN order_notes TEXT;

-- Payment Information
ALTER TABLE orders ADD COLUMN payment_method TEXT;
ALTER TABLE orders ADD COLUMN payment_intent_id TEXT; 