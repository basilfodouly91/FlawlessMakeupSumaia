# 📤 Where Are Uploaded Images Saved?

## ⚠️ **IMPORTANT: NO FILE STORAGE**

When you upload images through the admin panel, **they are NOT saved as separate files**.

Instead, uploaded images are:
- ✅ Converted to **Base64 strings**
- ✅ Stored **directly in the SQL Server database**
- ✅ Saved as **text** in the `ImageUrl` column

## 🔄 How Image Upload Works

### Step-by-Step Process:

```
1. Admin Panel (Browser)
   └─> You select an image file (product.jpg)
   
2. JavaScript (FileReader)
   └─> Converts file to Base64 string
   └─> "data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD..."
   
3. Angular Frontend
   └─> Sends Base64 string to API via HTTP POST
   
4. .NET Backend (ImageService)
   └─> Validates Base64 string
   └─> Ensures proper format
   
5. SQL Server Database
   └─> Saves Base64 string in Products.ImageUrl column
   └─> Stored as NVARCHAR(MAX) text
```

## 💾 Where Images Are Stored

### Database Information:
```
Server:   (localdb)\mssqllocaldb
Database: FlawlessMakeupDB
Table:    Products
Column:   ImageUrl (NVARCHAR(MAX))
```

### Example Database Record:

| Id | Name                  | ImageUrl                                        |
|----|----------------------|-------------------------------------------------|
| 1  | Foundation Cream     | data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ...   |
| 2  | Lipstick Red         | https://images.unsplash.com/photo-123...       |

## 📝 Base64 Image Format

### What does a Base64 image look like?

```
data:image/jpeg;base64,/9j/4AAQSkZJRgABAQEAYABgAAD/2wBDAAgGBgcGBQgHBwcJCQgKDBQN...
└───┬───┘ └───┬──┘ └──────────────────┬──────────────────┘
    │         │                       │
  Prefix   Format              Encoded Image Data
```

### Breakdown:
- **`data:`** - Data URI scheme prefix
- **`image/jpeg`** - MIME type (could be jpeg, png, gif, etc.)
- **`base64,`** - Encoding type
- **`/9j/4AAQ...`** - The actual image data encoded in Base64

## 📊 Storage Comparison

### Method 1: Base64 in Database (Current Method)
```
✅ Pros:
- Simple implementation
- No file system management
- Images included in database backups
- No broken file paths

❌ Cons:
- Increases database size (~33% larger than original)
- Slower queries
- Not ideal for many images
```

### Method 2: File Storage (Alternative)
```
✅ Pros:
- Small database size
- Fast queries
- Better for many images

❌ Cons:
- Need to manage file system
- Separate backup needed
- Can have broken file paths
```

## 🔍 How to View Uploaded Images

### 1. View in Database (SQL Query)
```sql
USE FlawlessMakeupDB;

SELECT 
    Id,
    Name,
    CASE 
        WHEN ImageUrl LIKE 'data:image%' THEN 'Base64 (Uploaded)'
        WHEN ImageUrl LIKE 'http%' THEN 'External URL'
        ELSE 'Unknown'
    END AS ImageType,
    LEFT(ImageUrl, 100) AS ImagePreview,
    LEN(ImageUrl) AS ImageSizeBytes
FROM Products;
```

### 2. View in Visual Studio
1. Open **SQL Server Object Explorer**
2. Connect to `(localdb)\mssqllocaldb`
3. Expand: `Databases` → `FlawlessMakeupDB` → `Tables` → `Products`
4. Right-click → **View Data**
5. Scroll to `ImageUrl` column
6. You'll see either:
   - `data:image/jpeg;base64,...` (uploaded images)
   - `https://...` (external URLs)

### 3. View on Website
Just visit your website at `http://localhost:4200` - the browser automatically converts Base64 back to images!

## 📂 No Physical Files

### There is NO folder with image files such as:
- ❌ `wwwroot/images/`
- ❌ `uploads/`
- ❌ `static/images/`
- ❌ `public/images/`

### Everything is in the database!

## 🔄 How to Extract Base64 Images to Files

If you want to convert Base64 images from the database to actual files:

### PowerShell Script:
```powershell
# Get products from API
$products = Invoke-RestMethod -Uri "http://localhost:5001/api/products"

# Create output folder
New-Item -ItemType Directory -Force -Path ".\ExtractedImages"

foreach ($product in $products) {
    # Check if it's a Base64 image
    if ($product.imageUrl -like "data:image*") {
        # Extract base64 data
        $base64 = $product.imageUrl -replace "^data:image/[^;]+;base64,", ""
        
        # Determine file extension
        if ($product.imageUrl -match "data:image/(\w+);") {
            $ext = $matches[1]
        } else {
            $ext = "jpg"
        }
        
        # Convert to bytes
        $bytes = [Convert]::FromBase64String($base64)
        
        # Save to file
        $fileName = "product_$($product.id).$ext"
        [System.IO.File]::WriteAllBytes(".\ExtractedImages\$fileName", $bytes)
        
        Write-Host "Extracted: $fileName ($([Math]::Round($bytes.Length / 1024, 2)) KB)"
    }
}

Write-Host "Done! Images saved to .\ExtractedImages\"
```

## 📏 Image Size Examples

### Original File vs Base64:

| Original Size | Base64 Size | Increase |
|--------------|-------------|----------|
| 100 KB       | 133 KB      | +33%     |
| 500 KB       | 667 KB      | +33%     |
| 1 MB         | 1.33 MB     | +33%     |
| 5 MB         | 6.67 MB     | +33%     |

**Base64 encoding adds ~33% to the file size!**

## 🎯 Current Configuration

### In Your Code:

**Frontend (product-management.ts):**
```typescript
onImageSelect(event: Event): void {
  const file = input.files[0];
  const reader = new FileReader();
  reader.onload = () => {
    const base64String = reader.result as string; // Base64 string
    this.productForm.imageUrl = base64String;     // Saved to form
  };
  reader.readAsDataURL(file);  // Convert to Base64
}
```

**Backend (ImageService.cs):**
```csharp
public string ProcessImage(string imageData)
{
    // Validates and ensures proper Base64 format
    if (imageData.StartsWith("data:image/"))
        return imageData;  // Saved as-is to database
}
```

**Database Schema:**
```sql
CREATE TABLE Products (
    Id INT PRIMARY KEY,
    Name NVARCHAR(200),
    ImageUrl NVARCHAR(MAX),  -- Base64 stored here as text
    ...
);
```

## ⚡ Performance Impact

### Database Size:
- 100 products with 500 KB images = **67 MB** in database
- 1000 products with 500 KB images = **667 MB** in database

### Query Performance:
- Larger database = slower queries
- Consider limiting images to **< 500 KB each**

## 🔧 Alternative: Store Files Instead

If you want to save images as actual files (recommended for production), you would need to:

1. Create an `wwwroot/images/products/` folder
2. Save uploaded files to that folder
3. Store only the file path in database: `/images/products/product123.jpg`
4. Update `ImageService` to handle file saving

**But currently, your app uses Base64 in database method.**

## 📞 Summary

### Quick Answer:
**Uploaded images are saved as Base64 text strings in the SQL Server database, specifically in the `Products.ImageUrl` column. There are no physical image files on disk.**

### Storage Path:
```
SQL Server Database
└── FlawlessMakeupDB
    └── Tables
        └── Products
            └── ImageUrl column (NVARCHAR(MAX))
                └── Base64 string: "data:image/jpeg;base64,/9j/4AAQ..."
```

### To View:
- **Website:** http://localhost:4200
- **Admin Panel:** http://localhost:4200/admin/products
- **Database:** Visual Studio → SQL Server Object Explorer
- **API:** http://localhost:5001/api/products

