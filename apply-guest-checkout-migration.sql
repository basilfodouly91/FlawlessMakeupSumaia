-- Manual migration script for guest checkout support
-- Run this if the automatic migration doesn't apply

-- Add GuestEmail column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'GuestEmail')
BEGIN
    ALTER TABLE Orders ADD GuestEmail NVARCHAR(MAX) NULL;
    PRINT 'Added GuestEmail column';
END
ELSE
BEGIN
    PRINT 'GuestEmail column already exists';
END

-- Add GuestName column if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'GuestName')
BEGIN
    ALTER TABLE Orders ADD GuestName NVARCHAR(MAX) NULL;
    PRINT 'Added GuestName column';
END
ELSE
BEGIN
    PRINT 'GuestName column already exists';
END

-- Make UserId nullable if it isn't already
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'UserId' AND is_nullable = 0)
BEGIN
    -- Drop the foreign key constraint if it exists
    DECLARE @constraintName NVARCHAR(200);
    SELECT @constraintName = name 
    FROM sys.foreign_keys 
    WHERE parent_object_id = OBJECT_ID('Orders') 
    AND referenced_object_id = OBJECT_ID('AspNetUsers');
    
    IF @constraintName IS NOT NULL
    BEGIN
        EXEC('ALTER TABLE Orders DROP CONSTRAINT ' + @constraintName);
    END
    
    -- Alter the column to be nullable
    ALTER TABLE Orders ALTER COLUMN UserId NVARCHAR(450) NULL;
    
    -- Re-create the foreign key constraint
    ALTER TABLE Orders ADD CONSTRAINT FK_Orders_AspNetUsers_UserId 
    FOREIGN KEY (UserId) REFERENCES AspNetUsers(Id);
    
    PRINT 'Made UserId nullable';
END
ELSE
BEGIN
    PRINT 'UserId is already nullable';
END

PRINT 'Guest checkout migration completed successfully!';

-- Verify the changes
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Orders'
AND COLUMN_NAME IN ('UserId', 'GuestEmail', 'GuestName')
ORDER BY COLUMN_NAME;

