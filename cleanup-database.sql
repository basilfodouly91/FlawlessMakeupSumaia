-- Cleanup script for ProductShades refactoring
USE FlawlessMakeupDB;
GO

-- Drop foreign key constraint if exists
IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_CartItems_ProductShades_ProductShadeId')
BEGIN
    ALTER TABLE CartItems DROP CONSTRAINT FK_CartItems_ProductShades_ProductShadeId;
    PRINT 'Dropped FK_CartItems_ProductShades_ProductShadeId';
END

-- Drop ProductShades table if exists
IF OBJECT_ID('dbo.ProductShades', 'U') IS NOT NULL
BEGIN
    DROP TABLE dbo.ProductShades;
    PRINT 'Dropped ProductShades table';
END

-- Drop AvailableShades column from Products if exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Products') AND name = 'AvailableShades')
BEGIN
    ALTER TABLE Products DROP COLUMN AvailableShades;
    PRINT 'Dropped AvailableShades column from Products';
END

-- Drop SelectedShade column from CartItems if exists  
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.CartItems') AND name = 'SelectedShade')
BEGIN
    ALTER TABLE CartItems DROP COLUMN SelectedShade;
    PRINT 'Dropped SelectedShade column from CartItems';
END

-- Drop ProductShadeId column from CartItems if exists
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.CartItems') AND name = 'ProductShadeId')
BEGIN
    ALTER TABLE CartItems DROP COLUMN ProductShadeId;
    PRINT 'Dropped ProductShadeId column from CartItems';
END

PRINT 'Database cleanup completed successfully!';
GO













