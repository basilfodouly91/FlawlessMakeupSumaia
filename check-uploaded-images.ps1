# Check if any images have been uploaded (Base64) vs External URLs

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  CHECKING IMAGE STORAGE TYPE" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Check if backend is running
try {
    $test = Invoke-RestMethod -Uri "http://localhost:5001/api/products" -ErrorAction Stop
} catch {
    Write-Host "Backend is not running!" -ForegroundColor Red
    Write-Host "Please start the backend first." -ForegroundColor Yellow
    exit
}

Write-Host "Analyzing image storage..." -ForegroundColor Yellow
Write-Host ""

# Get all products
$products = Invoke-RestMethod -Uri "http://localhost:5001/api/products"

# Count image types
$base64Count = 0
$urlCount = 0
$emptyCount = 0

foreach ($product in $products) {
    if ([string]::IsNullOrEmpty($product.imageUrl)) {
        $emptyCount++
    }
    elseif ($product.imageUrl -like "data:image*") {
        $base64Count++
    }
    else {
        $urlCount++
    }
}

# Display results
Write-Host "IMAGE STORAGE ANALYSIS:" -ForegroundColor Green
Write-Host ""
Write-Host "Total Products: $($products.Count)" -ForegroundColor White
Write-Host ""
Write-Host "Base64 (Uploaded to Database): $base64Count" -ForegroundColor Yellow
if ($base64Count -gt 0) {
    Write-Host "  - Images saved as Base64 in database" -ForegroundColor Green
} else {
    Write-Host "  - No uploaded images found" -ForegroundColor Gray
}
Write-Host ""

Write-Host "External URLs (from internet): $urlCount" -ForegroundColor Yellow
if ($urlCount -gt 0) {
    Write-Host "  - Images loaded from external sources" -ForegroundColor Green
}
Write-Host ""

Write-Host "No Image: $emptyCount" -ForegroundColor Yellow
Write-Host ""

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# Show examples
if ($base64Count -gt 0) {
    Write-Host "UPLOADED IMAGE EXAMPLES:" -ForegroundColor Green
    Write-Host ""
    
    $uploadedProducts = $products | Where-Object { $_.imageUrl -like "data:image*" } | Select-Object -First 3
    
    foreach ($product in $uploadedProducts) {
        $imageSize = [Math]::Round($product.imageUrl.Length / 1024, 2)
        Write-Host "Product: $($product.name)" -ForegroundColor White
        Write-Host "  Storage: Database (Base64)" -ForegroundColor Cyan
        Write-Host "  Size: $imageSize KB" -ForegroundColor Gray
        $preview = $product.imageUrl.Substring(0, [Math]::Min(60, $product.imageUrl.Length))
        Write-Host "  Preview: $preview..." -ForegroundColor DarkGray
        Write-Host ""
    }
    
    Write-Host "These images are stored IN THE DATABASE" -ForegroundColor Green
    Write-Host "Location: FlawlessMakeupDB -> Products -> ImageUrl" -ForegroundColor Yellow
    Write-Host ""
}

if ($urlCount -gt 0) {
    Write-Host "EXTERNAL URL EXAMPLES:" -ForegroundColor Blue
    Write-Host ""
    
    $urlProducts = $products | Where-Object { $_.imageUrl -notlike "data:image*" -and ![string]::IsNullOrEmpty($_.imageUrl) } | Select-Object -First 3
    
    foreach ($product in $urlProducts) {
        Write-Host "Product: $($product.name)" -ForegroundColor White
        Write-Host "  Storage: External URL" -ForegroundColor Cyan
        Write-Host "  URL: $($product.imageUrl)" -ForegroundColor Gray
        Write-Host ""
    }
    
    Write-Host "These images are loaded from internet" -ForegroundColor Blue
    Write-Host "Only the URL is stored in database" -ForegroundColor Yellow
    Write-Host ""
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "HOW TO UPLOAD IMAGES:" -ForegroundColor Yellow
Write-Host "  1. Go to: http://localhost:4200/admin/products" -ForegroundColor White
Write-Host "  2. Click 'Add New Product' or 'Edit'" -ForegroundColor White
Write-Host "  3. Click 'Upload Image' button" -ForegroundColor White
Write-Host "  4. Select JPG/PNG file (max 5MB)" -ForegroundColor White
Write-Host "  5. Image converted to Base64 and saved in database" -ForegroundColor White
Write-Host ""
Write-Host "WHERE UPLOADED IMAGES ARE SAVED:" -ForegroundColor Yellow
Write-Host "  Database: FlawlessMakeupDB" -ForegroundColor Cyan
Write-Host "  Table: Products" -ForegroundColor Cyan
Write-Host "  Column: ImageUrl (as Base64 text)" -ForegroundColor Cyan
Write-Host ""
Write-Host "Read: UPLOADED_IMAGES_EXPLAINED.md for details" -ForegroundColor Green
Write-Host ""
