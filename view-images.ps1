# Quick Image Viewer Script

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  IMAGE DATABASE VIEWER" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if API is running
try {
    $test = Invoke-RestMethod -Uri "http://localhost:5001/api/products" -ErrorAction Stop
} catch {
    Write-Host "❌ Backend is not running!" -ForegroundColor Red
    Write-Host "Please start the backend first." -ForegroundColor Yellow
    exit
}

# Get Products
Write-Host "📦 PRODUCTS WITH IMAGES:" -ForegroundColor Yellow
Write-Host ""

$products = Invoke-RestMethod -Uri "http://localhost:5001/api/products"

foreach ($product in $products | Select-Object -First 5) {
    Write-Host "Product: $($product.name)" -ForegroundColor White
    
    # Check image type
    if ($product.imageUrl -like "data:image*") {
        $imageType = "Base64 (stored in DB)"
        $imageSize = "$([Math]::Round($product.imageUrl.Length / 1024, 2)) KB"
    } else {
        $imageType = "External URL"
        $imageSize = "N/A"
    }
    
    Write-Host "  Type: $imageType" -ForegroundColor Cyan
    Write-Host "  Size: $imageSize" -ForegroundColor Gray
    
    # Show first 80 chars of URL
    $preview = $product.imageUrl.Substring(0, [Math]::Min(80, $product.imageUrl.Length))
    Write-Host "  URL:  $preview..." -ForegroundColor Gray
    Write-Host ""
}

Write-Host "---" -ForegroundColor Gray
Write-Host ""

# Get Categories
Write-Host "📁 CATEGORIES WITH IMAGES:" -ForegroundColor Yellow
Write-Host ""

$categories = Invoke-RestMethod -Uri "http://localhost:5001/api/categories"

foreach ($category in $categories | Select-Object -First 5) {
    Write-Host "Category: $($category.nameEn)" -ForegroundColor White
    
    # Check image type
    if ($category.imageUrl -like "data:image*") {
        $imageType = "Base64 (stored in DB)"
        $imageSize = "$([Math]::Round($category.imageUrl.Length / 1024, 2)) KB"
    } else {
        $imageType = "External URL"
        $imageSize = "N/A"
    }
    
    Write-Host "  Type: $imageType" -ForegroundColor Cyan
    Write-Host "  Size: $imageSize" -ForegroundColor Gray
    
    # Show first 80 chars of URL
    $preview = $category.imageUrl.Substring(0, [Math]::Min(80, $category.imageUrl.Length))
    Write-Host "  URL:  $preview..." -ForegroundColor Gray
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Summary
Write-Host "SUMMARY:" -ForegroundColor Yellow
Write-Host "  Total Products: $($products.Count)" -ForegroundColor White
Write-Host "  Total Categories: $($categories.Count)" -ForegroundColor White
Write-Host ""
Write-Host "To view all images, open:" -ForegroundColor Cyan
Write-Host "  Admin Panel: http://localhost:4200/admin/products" -ForegroundColor White
Write-Host "  Website: http://localhost:4200/products" -ForegroundColor White
Write-Host ""
Write-Host "To extract Base64 images, see: IMAGE_STORAGE_GUIDE.md" -ForegroundColor Yellow
Write-Host ""

