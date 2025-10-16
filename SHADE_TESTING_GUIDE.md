# Product Shade Management - Testing Guide

## 🌐 Access the Application

- **Frontend**: http://localhost:4200
- **Backend API**: https://localhost:5001
- **API Documentation**: https://localhost:5001/swagger

---

## 🧪 Step-by-Step Testing Guide

### Part 1: Admin - Add Shades to Product

1. **Login as Admin**
   - Navigate to: http://localhost:4200/login
   - Login with admin credentials

2. **Go to Product Management**
   - Click on "Admin" in header
   - Click on "Products" or navigate to: http://localhost:4200/admin/products

3. **Add Shades to a Product**
   - Click "Add New Product" or "Edit" an existing product
   - Scroll to the "Available Shades" section
   - Click the **"Add Shade"** button
   - Fill in shade details:
     ```
     Shade 1:
     - Name: 8B
     - Stock Quantity: 50
     - Active: ✓ checked
     ```
   - Click **"Add Shade"** again to add more:
     ```
     Shade 2:
     - Name: 12N
     - Stock Quantity: 30
     - Active: ✓ checked
     
     Shade 3:
     - Name: 12S Fair
     - Stock Quantity: 25
     - Active: ✓ checked
     
     Shade 4:
     - Name: 20S Light Sand
     - Stock Quantity: 0 (out of stock)
     - Active: ✓ checked
     
     Shade 5:
     - Name: 27H Light Medium Honey
     - Stock Quantity: 40
     - Active: ✓ checked
     ```
   - Use arrow buttons to reorder shades if needed
   - Click "Save"

4. **Verify Shades Were Saved**
   - Edit the product again
   - Verify all shades appear in the form
   - Check that display order is preserved

---

### Part 2: User - Select and Purchase Product with Shades

1. **Browse Products** (as regular user, no login needed)
   - Go to: http://localhost:4200
   - Click on the product you just added shades to

2. **View Product Detail Page**
   - Verify shade selection buttons appear
   - Notice that "Add to Cart" button is **disabled**
   - See red error message: "Please select a shade to continue"

3. **Select a Shade**
   - Click on any **available shade** (e.g., "8B")
   - Button should change to dark color when selected
   - Notice that "Add to Cart" button is now **enabled**
   - Try clicking on the out-of-stock shade (20S Light Sand)
   - It should be disabled and show "(Out of Stock)"

4. **Add to Cart**
   - With shade selected, click "Add to Cart"
   - Verify success confirmation appears
   - Click "View Cart"

5. **Verify Cart Shows Shade**
   - In cart, under the product name, you should see:
     ```
     Product Name
     Brand Name
     Shade: 8B
     ```

6. **Add Same Product with Different Shade**
   - Go back to the product
   - Select a different shade (e.g., "12N")
   - Add to cart
   - Return to cart
   - **Verify**: Both items appear separately:
     ```
     Product Name - Shade: 8B  (Quantity: 1)
     Product Name - Shade: 12N (Quantity: 1)
     ```

---

### Part 3: Advanced Testing

#### Test Guest Cart
1. Open incognito/private window
2. Add products with shades to cart
3. Verify shades are preserved in local storage
4. Login with your account
5. Verify guest cart items (with shades) transfer to user cart

#### Test Stock Validation
1. Add shade with 0 stock in admin
2. Verify it shows as "Out of Stock" on product page
3. Verify it's disabled and can't be selected

#### Test Products Without Shades
1. Create a product without adding any shades
2. Visit product detail page
3. Verify no shade selector appears
4. Verify "Add to Cart" works immediately without shade selection

#### Test Shade Reordering
1. Edit product with multiple shades
2. Use up/down arrows to reorder shades
3. Save product
4. Reload product detail page
5. Verify shades appear in new order

---

## 🎯 Expected Behavior

### Products WITH Shades:
- ✅ Shade selector appears on product detail page
- ✅ User MUST select a shade before adding to cart
- ✅ Add to Cart button disabled until shade selected
- ✅ Selected shade shows in cart
- ✅ Each shade creates separate cart item
- ✅ Out of stock shades are disabled

### Products WITHOUT Shades:
- ✅ No shade selector appears
- ✅ Add to Cart works immediately
- ✅ Functions like normal product

### Admin Panel:
- ✅ Can add unlimited shades to any product
- ✅ Can set individual stock per shade
- ✅ Can reorder shades
- ✅ Can activate/deactivate specific shades
- ✅ Can delete shades
- ✅ Products can have zero shades (optional)

---

## 📊 Sample Data

### Example Shade Names (from Tarte Shape Tape):
- 8B
- 12N
- 12S Fair
- 16N Fair Light Neutral
- 20B Light
- 20S Light Sand
- 22B Light Beige
- 22N Light Neutral
- 27B Light Medium Beige
- 27H Light Medium Honey
- 27S Light Medium Sand
- 29N Light Medium
- 34S Medium Sand
- 35H Medium Honey
- 35N Medium
- 36S Medium Tan Sand
- 38N Medium Tan Neutral
- 42S Tan Sand
- 44H Tan Honey

---

## 🚀 Quick Start Commands

```powershell
# Start SQL Server LocalDB (already running)
sqllocaldb start MSSQLLocalDB

# Backend (API)
cd FlawlessMakeupSumaia.API
dotnet run

# Frontend (Angular)
cd FlawlessMakeupSumaia.Client
npm start
```

---

## 🔧 Troubleshooting

### Issue: Shades not appearing
- **Solution**: Check browser console for errors, verify product was saved with shades

### Issue: Can't add to cart even with shade selected
- **Solution**: Verify shade has stock > 0, check network tab for API errors

### Issue: Shades not saving
- **Solution**: Ensure shade name is filled, stock quantity is >= 0

### Issue: Migration errors
- **Solution**: Database already updated successfully using LocalDB

---

## ✨ Implementation Complete!

All features have been implemented according to the All Original Brands reference website. The system now supports:
- Professional shade management
- Individual stock tracking per shade
- Dynamic admin UI
- User-friendly shade selection
- Cart integration with shade tracking










