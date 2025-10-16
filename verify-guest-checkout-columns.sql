-- Verify if guest checkout columns exist in Orders table

SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Orders'
AND COLUMN_NAME IN ('UserId', 'GuestEmail', 'GuestName')
ORDER BY COLUMN_NAME;

-- If you see all three columns (UserId, GuestEmail, GuestName), the migration is complete!
-- If GuestEmail or GuestName are missing, run: apply-guest-checkout-migration.sql

