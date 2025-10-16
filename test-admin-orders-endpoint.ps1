# Test Admin Orders Endpoint
# This script tests if the admin orders API endpoint is working

Write-Host "=== Testing Admin Orders API Endpoint ===" -ForegroundColor Cyan
Write-Host ""

# Check if API is running
Write-Host "1. Checking if API is running..." -ForegroundColor Yellow
try {
    $apiHealth = Invoke-WebRequest -Uri "http://localhost:5000/api/products" -Method GET -UseBasicParsing -ErrorAction Stop
    Write-Host "   ✓ API is running!" -ForegroundColor Green
} catch {
    Write-Host "   ✗ API is NOT running!" -ForegroundColor Red
    Write-Host "   Please start the API first:" -ForegroundColor Yellow
    Write-Host "   cd FlawlessMakeupSumaia.API" -ForegroundColor White
    Write-Host "   dotnet run" -ForegroundColor White
    exit
}

Write-Host ""

# Ask for JWT token
Write-Host "2. To test the admin orders endpoint, we need your JWT token" -ForegroundColor Yellow
Write-Host "   How to get your token:" -ForegroundColor Cyan
Write-Host "   - Open http://localhost:4200 in browser" -ForegroundColor White
Write-Host "   - Press F12 -> Application tab -> Local Storage -> http://localhost:4200" -ForegroundColor White
Write-Host "   - Copy the 'token' value" -ForegroundColor White
Write-Host ""
Write-Host "Paste your JWT token here (or press Enter to skip): " -ForegroundColor Yellow -NoNewline
$token = Read-Host

if ($token) {
    Write-Host ""
    Write-Host "3. Testing admin orders endpoint..." -ForegroundColor Yellow
    
    try {
        $headers = @{
            "Authorization" = "Bearer $token"
        }
        
        $response = Invoke-RestMethod -Uri "http://localhost:5000/api/orders/admin/all" -Headers $headers -Method GET
        
        Write-Host "   ✓ Admin orders endpoint works!" -ForegroundColor Green
        Write-Host ""
        Write-Host "   Orders found: $($response.Count)" -ForegroundColor Cyan
        Write-Host ""
        
        if ($response.Count -gt 0) {
            Write-Host "   Order Details:" -ForegroundColor Cyan
            foreach ($order in $response) {
                Write-Host "   - Order #$($order.orderNumber)" -ForegroundColor White
                Write-Host "     Customer: $($order.shippingFirstName) $($order.shippingLastName)" -ForegroundColor White
                Write-Host "     Status: $($order.status)" -ForegroundColor White
                Write-Host "     Total: $($order.totalAmount) JOD" -ForegroundColor White
                Write-Host "     Guest: $($order.guestEmail -or 'N/A')" -ForegroundColor White
                Write-Host ""
            }
        } else {
            Write-Host "   No orders returned from API" -ForegroundColor Yellow
        }
        
    } catch {
        Write-Host "   ✗ Error calling admin orders endpoint!" -ForegroundColor Red
        Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        
        if ($_.Exception.Message -like "*401*" -or $_.Exception.Message -like "*Unauthorized*") {
            Write-Host "   This looks like an authentication error." -ForegroundColor Yellow
            Write-Host "   Make sure you're logged in as an admin user." -ForegroundColor Yellow
        }
        
        if ($_.Exception.Message -like "*403*" -or $_.Exception.Message -like "*Forbidden*") {
            Write-Host "   This looks like an authorization error." -ForegroundColor Yellow
            Write-Host "   Make sure your user has the 'Admin' role." -ForegroundColor Yellow
        }
    }
} else {
    Write-Host ""
    Write-Host "Skipped API test. Get your token and run this script again." -ForegroundColor Yellow
}

Write-Host ""
Write-Host "=== Quick Fixes ===" -ForegroundColor Cyan
Write-Host "1. Make sure API is running: cd FlawlessMakeupSumaia.API && dotnet run" -ForegroundColor White
Write-Host "2. Make sure you're logged in as admin in the browser" -ForegroundColor White
Write-Host "3. Check browser console (F12) for errors" -ForegroundColor White
Write-Host "4. Check Network tab (F12) to see if API call is being made" -ForegroundColor White
Write-Host ""

