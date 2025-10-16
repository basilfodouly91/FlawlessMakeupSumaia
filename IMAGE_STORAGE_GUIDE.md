# Image Storage Guide

## 📸 Where Are Images Stored?

Images in your application are stored in the **SQL Server database** as **text strings** in the following tables:

### Tables with Images:
1. **Products** table → `ImageUrl` column (main image)
2. **Products** table → `ImageUrls` column (additional images, stored as JSON)
3. **Categories** table → `ImageUrl` column

## 📊 How to View Images in the Database

### Option 1: Using SQL Server Management Studio (SSMS)

1. **Connect to LocalDB**
   ```
   Server: (localdb)\mssqllocaldb
   Database: FlawlessMakeupDB
   Authentication: Windows Authentication
   ```

2. **Query Products Images**
   ```sql
   SELECT 
       Id,
       Name,
       ImageUrl,
       ImageUrls
   FROM Products
   ```

3. **Query Categories Images**
   ```sql
   SELECT 
       Id,
       NameEn,
       ImageUrl
   FROM Categories
   ```

### Option 2: Using Visual Studio SQL Server Object Explorer

1. Open Visual Studio
2. Go to **View** → **SQL Server Object Explorer**
3. Expand **(localdb)\MSSQLLocalDB**
4. Expand **Databases** → **FlawlessMakeupDB**
5. Expand **Tables**
6. Right-click **Products** → **View Data**
7. Scroll to see the `ImageUrl` column

### Option 3: Using PowerShell

```powershell
# Query Products with Images
$connectionString = "Server=(localdb)\mssqllocaldb;Database=FlawlessMakeupDB;Trusted_Connection=True;"
$query = "SELECT TOP 5 Id, Name, ImageUrl FROM Products"

$connection = New-Object System.Data.SqlClient.SqlConnection($connectionString)
$command = New-Object System.Data.SqlClient.SqlCommand($query, $connection)

$connection.Open()
$reader = $command.ExecuteReader()

while ($reader.Read()) {
    Write-Host "Product: $($reader['Name'])"
    Write-Host "Image: $($reader['ImageUrl'])"
    Write-Host "---"
}

$connection.Close()
```

### Option 4: Using the API (Current Data)

```powershell
# Get all products with images
$products = Invoke-RestMethod -Uri "http://localhost:5001/api/products"
$products | Select-Object name, imageUrl | Format-Table

# Get all categories with images
$categories = Invoke-RestMethod -Uri "http://localhost:5001/api/categories"
$categories | Select-Object nameEn, imageUrl | Format-Table
```

## 🖼️ Image Storage Types

### 1. External URLs (Current Method)
**Example:**
```
https://images.unsplash.com/photo-1631214540242-6b1e5b3c8e9b?w=400
```

**Pros:**
- ✅ Small database size
- ✅ Fast loading
- ✅ No storage costs

**Cons:**
- ❌ Depends on external service
- ❌ Images can be removed by host

### 2. Base64 Strings (When Using File Upload)
**Example:**
```
data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD/2wBD...
```

**Pros:**
- ✅ Images stored in database
- ✅ No external dependencies
- ✅ Complete control

**Cons:**
- ❌ Large database size
- ❌ Slower queries
- ❌ Can slow down backups

## 📥 How to Extract Base64 Images

If you have Base64 images and want to save them as files:

### PowerShell Script to Extract Images:

```powershell
# Create output directory
New-Item -ItemType Directory -Force -Path ".\ExtractedImages"

# Get products from API
$products = Invoke-RestMethod -Uri "http://localhost:5001/api/products"

foreach ($product in $products) {
    if ($product.imageUrl -and $product.imageUrl.StartsWith("data:image")) {
        # Extract Base64 data
        $base64 = $product.imageUrl -replace "^data:image/[^;]+;base64,", ""
        
        # Determine file extension
        if ($product.imageUrl -match "data:image/(\w+);") {
            $ext = $matches[1]
        } else {
            $ext = "jpg"
        }
        
        # Save to file
        $fileName = "product_$($product.id).$ext"
        $bytes = [Convert]::FromBase64String($base64)
        [System.IO.File]::WriteAllBytes(".\ExtractedImages\$fileName", $bytes)
        
        Write-Host "Extracted: $fileName"
    }
}
```

## 🔄 How to Change Image Storage

### Current Storage: External URLs

To change to file storage:

1. **Upload images using the admin panel**
   - Go to: http://localhost:4200/admin/products
   - Click "Add New Product" or "Edit"
   - Use the "Upload Image" button
   - Select image file (JPG/PNG, max 5MB)
   - Images will be stored as Base64 in database

2. **Or store files on server** (recommended for production)
   - Create a folder: `wwwroot/images/products`
   - Save uploaded files there
   - Store only the file path in database: `/images/products/product123.jpg`

## 📂 Database Schema

### Products Table - Image Columns:
```sql
ImageUrl    NVARCHAR(MAX)  -- Main product image
ImageUrls   NVARCHAR(MAX)  -- Additional images (JSON array)
```

### Categories Table - Image Column:
```sql
ImageUrl    NVARCHAR(MAX)  -- Category image
```

## 🛠️ Useful SQL Queries

### Find Products with External URLs:
```sql
SELECT Id, Name, ImageUrl
FROM Products
WHERE ImageUrl LIKE 'http%'
```

### Find Products with Base64 Images:
```sql
SELECT Id, Name, 
       LEFT(ImageUrl, 50) AS ImagePreview,
       LEN(ImageUrl) AS ImageSize
FROM Products
WHERE ImageUrl LIKE 'data:image%'
```

### Count Image Types:
```sql
SELECT 
    CASE 
        WHEN ImageUrl LIKE 'http%' THEN 'External URL'
        WHEN ImageUrl LIKE 'data:image%' THEN 'Base64'
        ELSE 'Unknown'
    END AS ImageType,
    COUNT(*) AS Count
FROM Products
GROUP BY 
    CASE 
        WHEN ImageUrl LIKE 'http%' THEN 'External URL'
        WHEN ImageUrl LIKE 'data:image%' THEN 'Base64'
        ELSE 'Unknown'
    END
```

### Get Database Size (including images):
```sql
SELECT 
    name AS DatabaseName,
    size * 8.0 / 1024 AS SizeMB
FROM sys.master_files
WHERE database_id = DB_ID('FlawlessMakeupDB')
```

## 📱 Viewing Images in Browser

### Via API:
```
http://localhost:5001/api/products/{id}
```

### In Admin Panel:
```
http://localhost:4200/admin/products
```

### On Website:
```
http://localhost:4200/products
```

## 💡 Best Practices

### For Development:
- ✅ Use external URLs (Unsplash, etc.)
- ✅ Small database size
- ✅ Easy testing

### For Production:
- ✅ Use Azure Blob Storage or AWS S3
- ✅ Store file path/URL in database
- ✅ Fast, scalable, and cost-effective

### For Small Projects:
- ✅ Base64 in database (current file upload)
- ✅ Simple, no external dependencies
- ✅ Easy backup (all in one database)

## 🔍 Current Image Sources

All current images are from **Unsplash** (free stock photos):
- Categories: `https://images.unsplash.com/...`
- Products: `https://images.unsplash.com/...`

These are high-quality placeholder images that you can replace with actual product photos using the admin panel.

## ⚠️ Important Notes

1. **Base64 images increase database size significantly**
   - 1 MB image = ~1.4 MB in Base64
   - 100 products with 1 MB images = ~140 MB database

2. **LocalDB has a 10 GB size limit**
   - Monitor your database size
   - Consider file storage for many images

3. **Backup includes images**
   - If using Base64, backups will be large
   - External URLs = small backups

## 📞 Need Help?

To view images right now:
```powershell
# Quick view of all product images
Invoke-RestMethod -Uri "http://localhost:5001/api/products" | 
    Select-Object name, imageUrl | 
    Format-Table -AutoSize
```

