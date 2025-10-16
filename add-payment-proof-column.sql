-- Add PaymentProofImageUrl column to Orders table

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Orders]') AND name = 'PaymentProofImageUrl')
BEGIN
    ALTER TABLE Orders ADD PaymentProofImageUrl NVARCHAR(MAX) NULL;
    PRINT 'Added PaymentProofImageUrl column';
END
ELSE
BEGIN
    PRINT 'PaymentProofImageUrl column already exists';
END

-- Verify the change
SELECT 
    COLUMN_NAME, 
    DATA_TYPE, 
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS
WHERE TABLE_NAME = 'Orders'
AND COLUMN_NAME = 'PaymentProofImageUrl';

