-- Verify Image Data in Database
-- Run this in SQL Server Management Studio or Visual Studio

USE FlawlessMakeupDB;
GO

-- ======================================
-- CHECK FOUNDATION PRODUCT
-- ======================================

-- Get foundation product info
SELECT 
    Id,
    Name,
    CASE 
        WHEN ImageUrl IS NULL THEN 'NULL'
        WHEN ImageUrl = '' THEN 'EMPTY STRING'
        WHEN ImageUrl LIKE 'data:image%' THEN 'Base64 (UPLOADED IMAGE)'
        WHEN ImageUrl LIKE 'http%' THEN 'External URL'
        ELSE 'Unknown'
    END AS ImageType,
    LEN(ImageUrl) AS ImageSizeInCharacters,
    LEN(ImageUrl) / 1024 AS ImageSizeInKB,
    LEFT(ImageUrl, 100) AS ImagePreview
FROM Products
WHERE Name LIKE '%foundation%'
ORDER BY Id;

-- ======================================
-- DETAILED CHECK FOR PRODUCT ID 18
-- ======================================

-- This will show that the data EXISTS
SELECT 
    Id,
    Name,
    CASE 
        WHEN LEN(ImageUrl) > 0 THEN 'DATA EXISTS ✓'
        ELSE 'NO DATA ✗'
    END AS Status,
    LEN(ImageUrl) AS TotalCharacters,
    SUBSTRING(ImageUrl, 1, 50) AS First50Chars,
    SUBSTRING(ImageUrl, LEN(ImageUrl) - 50, 50) AS Last50Chars
FROM Products
WHERE Id = 18;

-- ======================================
-- CHECK ALL PRODUCTS WITH BASE64
-- ======================================

SELECT 
    Id,
    Name,
    'Base64 Image' AS Type,
    LEN(ImageUrl) AS SizeInCharacters,
    CAST(LEN(ImageUrl) / 1024.0 AS DECIMAL(10,2)) AS SizeInKB
FROM Products
WHERE ImageUrl LIKE 'data:image%'
ORDER BY LEN(ImageUrl) DESC;

-- ======================================
-- VERIFY DATA IS NOT NULL
-- ======================================

SELECT 
    CASE 
        WHEN EXISTS (
            SELECT 1 
            FROM Products 
            WHERE Id = 18 
            AND ImageUrl IS NOT NULL 
            AND LEN(ImageUrl) > 100000
        ) THEN '✓ IMAGE DATA EXISTS IN DATABASE'
        ELSE '✗ NO IMAGE DATA FOUND'
    END AS VerificationResult;

