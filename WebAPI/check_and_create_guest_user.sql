-- Check if user with ID 1 exists
SELECT COUNT(*) as user_count FROM users WHERE id = 1;

-- Create a guest user with ID 1 if it doesn't exist
INSERT INTO users (id, first_name, last_name, email, password_hash, password_salt, created_at, updated_at, is_active, email_verified)
SELECT 1, 'Guest', 'User', 'guest@example.com', 'dummy_hash', 'dummy_salt', datetime('now'), datetime('now'), 1, 1
WHERE NOT EXISTS (SELECT 1 FROM users WHERE id = 1); 