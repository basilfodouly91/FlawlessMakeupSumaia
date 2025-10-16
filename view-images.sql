-- View Images from Database
-- Run this in SQL Server Management Studio or Visual Studio

USE FlawlessMakeupDB;
GO

-- ======================================
-- PRODUCTS WITH IMAGES
-- ======================================

SELECT TOP 10
    Id,
    Name AS ProductName,
    Brand,
    Price,
    CASE 
        WHEN ImageUrl LIKE 'data:image%' THEN 'Base64 (Stored in DB)'
        WHEN ImageUrl LIKE 'http%' THEN 'External URL'
        ELSE 'Unknown'
    END AS ImageType,
    LEFT(ImageUrl, 100) AS ImagePreview,
    LEN(ImageUrl) AS ImageSize
FROM Products
ORDER BY Id;

-- ======================================
-- CATEGORIES WITH IMAGES
-- ======================================

SELECT 
    Id,
    NameEn AS CategoryName,
    NameAr AS CategoryNameArabic,
    CASE 
        WHEN ImageUrl LIKE 'data:image%' THEN 'Base64 (Stored in DB)'
        WHEN ImageUrl LIKE 'http%' THEN 'External URL'
        ELSE 'Unknown'
    END AS ImageType,
    ImageUrl
FROM Categories
ORDER BY DisplayOrder;

-- ======================================
-- IMAGE STORAGE STATISTICS
-- ======================================

SELECT 
    'Products' AS TableName,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN ImageUrl LIKE 'data:image%' THEN 1 ELSE 0 END) AS Base64Images,
    SUM(CASE WHEN ImageUrl LIKE 'http%' THEN 1 ELSE 0 END) AS ExternalURLs,
    AVG(LEN(ImageUrl)) AS AvgImageSize
FROM Products

UNION ALL

SELECT 
    'Categories' AS TableName,
    COUNT(*) AS TotalRecords,
    SUM(CASE WHEN ImageUrl LIKE 'data:image%' THEN 1 ELSE 0 END) AS Base64Images,
    SUM(CASE WHEN ImageUrl LIKE 'http%' THEN 1 ELSE 0 END) AS ExternalURLs,
    AVG(LEN(ImageUrl)) AS AvgImageSize
FROM Categories;

-- ======================================
-- DATABASE SIZE (including images)
-- ======================================

SELECT 
    DB_NAME() AS DatabaseName,
    SUM(size * 8.0 / 1024) AS SizeMB,
    SUM(size * 8.0 / 1024 / 1024) AS SizeGB
FROM sys.master_files
WHERE database_id = DB_ID('FlawlessMakeupDB')
GROUP BY database_id;

-- ======================================
-- FIND PRODUCTS WITHOUT IMAGES
-- ======================================

SELECT 
    Id,
    Name,
    'No Image' AS Issue
FROM Products
WHERE ImageUrl IS NULL OR ImageUrl = '';

-- ======================================
-- FIND CATEGORIES WITHOUT IMAGES
-- ======================================

SELECT 
    Id,
    NameEn,
    'No Image' AS Issue
FROM Categories
WHERE ImageUrl IS NULL OR ImageUrl = '';

