# Add Categories Script
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "`n=== ADDING CATEGORIES ===" -ForegroundColor Cyan

try {
    Write-Host "Logging in as admin..." -ForegroundColor Yellow
    $loginData = '{"email":"admin@flawlessmakeup.com","password":"Admin@123"}'
    $loginResult = Invoke-RestMethod -Uri "http://localhost:5001/api/auth/login" -Method POST -Body $loginData -ContentType "application/json;charset=utf-8"
    Write-Host "Login successful!`n" -ForegroundColor Green
    
    $headers = @{
        "Authorization" = "Bearer $($loginResult.token)"
        "Content-Type" = "application/json;charset=utf-8"
    }
    
    $categories = @(
        '{"nameEn":"MakeUp","nameAr":"مكياج","description":"Complete makeup collection including lipsticks, foundations, eyeshadows and more","imageUrl":"https://images.unsplash.com/photo-1512496015851-a90fb38ba796?w=400","displayOrder":1,"isActive":true}',
        '{"nameEn":"Skin Care","nameAr":"العناية بالبشرة","description":"Premium skincare products for all skin types","imageUrl":"https://images.unsplash.com/photo-1556228720-195a672e8a03?w=400","displayOrder":2,"isActive":true}',
        '{"nameEn":"Fragrance","nameAr":"العطور","description":"Luxury perfumes and fragrances","imageUrl":"https://images.unsplash.com/photo-1541643600914-78b084683601?w=400","displayOrder":3,"isActive":true}',
        '{"nameEn":"Hair Care","nameAr":"العناية بالشعر","description":"Professional hair care products and treatments","imageUrl":"https://images.unsplash.com/photo-1522338242992-e1a54906a8da?w=400","displayOrder":4,"isActive":true}',
        '{"nameEn":"Body Care","nameAr":"العناية بالجسم","description":"Body lotions, oils and care essentials","imageUrl":"https://images.unsplash.com/photo-1608248597279-f99d160bfcbc?w=400","displayOrder":5,"isActive":true}',
        '{"nameEn":"Lash/Brow Care","nameAr":"العناية بالرموش والحواجب","description":"Lash and brow enhancement products","imageUrl":"https://images.unsplash.com/photo-1583001931096-959e6a6a6a1d?w=400","displayOrder":6,"isActive":true}',
        '{"nameEn":"Teeth Care","nameAr":"العناية بالأسنان","description":"Teeth whitening and oral care products","imageUrl":"https://images.unsplash.com/photo-1606811971618-4486d14f3f99?w=400","displayOrder":7,"isActive":true}',
        '{"nameEn":"Trendy Original Products","nameAr":"منتجات أصلية رائجة","description":"Latest trending and original beauty products","imageUrl":"https://images.unsplash.com/photo-1596462502278-27bfdc403348?w=400","displayOrder":8,"isActive":true}'
    )
    
    $names = @(
        @{en="MakeUp"; ar="مكياج"},
        @{en="Skin Care"; ar="العناية بالبشرة"},
        @{en="Fragrance"; ar="العطور"},
        @{en="Hair Care"; ar="العناية بالشعر"},
        @{en="Body Care"; ar="العناية بالجسم"},
        @{en="Lash/Brow Care"; ar="العناية بالرموش والحواجب"},
        @{en="Teeth Care"; ar="العناية بالأسنان"},
        @{en="Trendy Original Products"; ar="منتجات أصلية رائجة"}
    )
    
    $count = 0
    for ($i = 0; $i -lt $categories.Length; $i++) {
        $result = Invoke-RestMethod -Uri "http://localhost:5001/api/categories" -Method POST -Body $categories[$i] -Headers $headers
        $count++
        Write-Host "$count. Added: $($names[$i].en) / $($names[$i].ar)" -ForegroundColor Green
    }
    
    Write-Host "`n✅ Successfully added all $count categories!" -ForegroundColor Green
    Write-Host "View them at: http://localhost:4200/admin/categories`n" -ForegroundColor Cyan
    
} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.ErrorDetails) {
        Write-Host "Details: $($_.ErrorDetails.Message)" -ForegroundColor Yellow
    }
}








