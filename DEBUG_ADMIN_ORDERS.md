# Debug Admin Orders Not Showing

## Quick Diagnosis Steps

### 1. Check if API is Running
- The API should be running on http://localhost:5000 or http://localhost:5186
- Open a new PowerShell window and run:
  ```powershell
  cd FlawlessMakeupSumaia.API
  dotnet run
  ```
- Look for: "Now listening on: http://localhost:XXXX"

### 2. Check Browser Console
- Open http://localhost:4200/admin/orders
- Press F12 to open Developer Tools
- Go to Console tab
- Look for errors (especially 401 Unauthorized, 403 Forbidden, 404 Not Found, CORS errors)

### 3. Verify You're Logged In as Admin
- Check if you're logged in
- Check if your user has Admin role
- Try logging out and logging back in

### 4. Test API Endpoint Directly
Open a new PowerShell window and test the API:

```powershell
# Test if orders endpoint works (replace YOUR_TOKEN with actual token)
$token = "YOUR_JWT_TOKEN_HERE"
$headers = @{
    "Authorization" = "Bearer $token"
}
Invoke-RestMethod -Uri "http://localhost:5000/api/orders/admin/all" -Headers $headers
```

To get your token:
- F12 in browser
- Go to Application tab -> Local Storage -> http://localhost:4200
- Copy the value of "token"

### 5. Check Network Tab
- F12 -> Network tab
- Go to /admin/orders page
- Look for the API call to "api/orders/admin/all"
- Check:
  - Status code (should be 200)
  - Response (should have orders array)
  - Request headers (should have Authorization: Bearer token)

## Common Issues & Solutions

### Issue 1: API Not Running
**Symptom:** Network errors, "Connection refused"
**Solution:** Start the API server:
```powershell
cd FlawlessMakeupSumaia.API
dotnet run
```

### Issue 2: Not Logged In as Admin
**Symptom:** 401 Unauthorized or 403 Forbidden
**Solution:** 
- Make sure you're logged in
- Verify admin credentials in database
- Default admin: Check DbSeeder.cs for credentials

### Issue 3: CORS Error
**Symptom:** "CORS policy" error in console
**Solution:** Already configured in Program.cs, but verify API is running

### Issue 4: Orders Endpoint Returns Empty Array
**Symptom:** API returns [] but database has orders
**Solution:** Check if the orders were created with the new schema
- Run: `verify-guest-checkout-columns.sql` to check columns exist

### Issue 5: Frontend Error
**Symptom:** TypeScript errors in console
**Solution:** Check Angular terminal for compilation errors

## Manual Test: Create a Test Order

Run this SQL to create a test order:

```sql
INSERT INTO Orders (
    OrderNumber, 
    OrderDate, 
    Status, 
    SubTotal, 
    Tax, 
    ShippingCost, 
    TotalAmount,
    ShippingFirstName,
    ShippingLastName,
    ShippingAddress,
    ShippingCity,
    ShippingState,
    ShippingZipCode,
    ShippingCountry,
    ShippingPhone,
    PaymentMethod,
    GuestEmail,
    GuestName,
    Notes
) VALUES (
    '202510160002',
    GETDATE(),
    0, -- Pending
    100.00,
    0,
    3.00,
    103.00,
    'Test',
    'Customer',
    '123 Test Street',
    'Amman',
    'Amman',
    '11183',
    'Jordan',
    '0791234567',
    'Cash on Delivery',
    'test@example.com',
    'Test Customer',
    'Test order for debugging'
);
```

## Expected Behavior

When you visit /admin/orders, you should see:
1. A table with all orders
2. Order number, customer name, date, total, status
3. Dropdown to filter by status
4. Dropdown to update each order's status
5. "View Details" button for each order

## Still Not Working?

Share the following info:
1. Browser console errors (F12 -> Console tab)
2. Network tab response for "api/orders/admin/all" call
3. API console output when accessing the orders page

