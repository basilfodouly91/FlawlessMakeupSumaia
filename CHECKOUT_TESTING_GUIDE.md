# Checkout and Order Management Testing Guide

## How to Check the Changes

### 1. View the Angular Application

The Angular dev server is already running at:
- **URL:** http://localhost:4200
- Open your browser and navigate to this URL

### 2. Test Checkout Flow (Guest User)

1. **Add Products to Cart**
   - Browse products on the home page or /products
   - Click "Add to Cart" on any product
   - View cart at http://localhost:4200/cart

2. **Proceed to Checkout**
   - Click "Proceed to Checkout" button
   - You'll be redirected to http://localhost:4200/checkout

3. **Fill Checkout Form as Guest**
   - Enter Email (required for guest)
   - Enter Guest Name (required for guest)
   - Fill in shipping information:
     - First Name *
     - Last Name *
     - Phone *
     - City *
     - Address *
     - Delivery Notes (optional)

4. **Payment Information**
   - Payment method is "Cash on Delivery" (default)
   - You'll see CliQ payment info: "CliQ: BASILFODOULY"
   - Click the "Copy CliQ" button to test copy functionality

5. **Review Order Summary**
   - Check cart items on the right side
   - Verify subtotal calculation
   - Verify shipping cost is 3 JOD
   - Verify total = subtotal + 3 JOD

6. **Place Order**
   - Click "Place Order" button
   - You'll be redirected to order confirmation page

7. **Order Confirmation**
   - View order number
   - See order details, items, and totals
   - CliQ payment info is displayed again
   - Click "Continue Shopping" to go back to home

### 3. Test Checkout Flow (Logged-in User)

1. **Login First**
   - Go to http://localhost:4200/login
   - Login with your account

2. **Add Products and Checkout**
   - Add products to cart
   - Go to checkout
   - Notice: No guest email/name fields (you're logged in)
   - Fill shipping information
   - Place order
   - View confirmation

### 4. Test Admin Order Management

1. **Login as Admin**
   - Go to http://localhost:4200/login
   - Login with admin credentials

2. **Access Admin Dashboard**
   - Go to http://localhost:4200/admin
   - You'll see the new "Orders" card

3. **View Order Management**
   - Click "Order Management" button
   - You'll be redirected to http://localhost:4200/admin/orders

4. **Admin Features to Test:**
   - **View All Orders:** See all orders in a table
   - **Filter by Status:** Use dropdown to filter (All, Pending, Confirmed, Completed, Cancelled)
   - **Update Order Status:** Change status using dropdown in each row
   - **View Details:** Click "View Details" to see order details in modal
   - **Guest Indicator:** Guest orders show "Guest" badge
   - **Customer Info:** See customer email, phone, and address

### 5. Check Email Notifications

**Note:** Email functionality requires SMTP configuration

1. **Configure Email Settings**
   - Open `FlawlessMakeupSumaia.API/appsettings.json`
   - Update the Email section:
     ```json
     "Email": {
       "AdminEmail": "YOUR_EMAIL@example.com",
       "SmtpHost": "smtp.gmail.com",
       "SmtpPort": "587",
       "SmtpUsername": "YOUR_GMAIL@gmail.com",
       "SmtpPassword": "YOUR_APP_PASSWORD",
       "FromEmail": "noreply@flawlessmakeup.com",
       "FromName": "Flawless Makeup Sumaia"
     }
     ```

2. **For Gmail Users:**
   - Enable 2-factor authentication
   - Generate an App Password: https://myaccount.google.com/apppasswords
   - Use the App Password in SmtpPassword field

3. **Test Email:**
   - Place a test order
   - Check the admin email inbox
   - You should receive an HTML email with:
     - Order number
     - Customer information
     - Order items table
     - Shipping address
     - Payment method
     - CliQ info
     - Total amount

### 6. Apply Database Migration

**Before testing the backend fully, you need to apply the migration:**

1. **Stop the API Server** (if running)

2. **Run Migration Commands:**
   ```powershell
   cd FlawlessMakeupSumaia.API
   dotnet ef migrations add UpdateOrderForGuestCheckout
   dotnet ef database update
   ```

3. **Restart API Server:**
   ```powershell
   dotnet run
   ```

### 7. Verify All Features

**Checklist:**
- ✅ Guest checkout works without login
- ✅ Logged-in user checkout works
- ✅ Shipping is fixed at 3 JOD
- ✅ No tax is added
- ✅ CliQ info displays and copy button works
- ✅ Order confirmation page shows correctly
- ✅ Admin can view all orders
- ✅ Admin can filter orders by status
- ✅ Admin can update order status
- ✅ Admin can view order details
- ✅ Email notification sent to admin (if SMTP configured)
- ✅ Both English and Arabic translations work

## Quick URLs

- **Home:** http://localhost:4200/
- **Products:** http://localhost:4200/products
- **Cart:** http://localhost:4200/cart
- **Checkout:** http://localhost:4200/checkout
- **Login:** http://localhost:4200/login
- **Admin Dashboard:** http://localhost:4200/admin
- **Admin Orders:** http://localhost:4200/admin/orders

## Troubleshooting

### Angular Server Not Showing Changes
- The server auto-recompiles on file changes
- Check terminal for compilation errors
- Refresh your browser (Ctrl+Shift+R for hard refresh)

### API Not Working
- Make sure API is running
- Check that migration was applied
- Verify CORS is enabled for Angular app

### Email Not Sending
- Check SMTP configuration in appsettings.json
- For Gmail: ensure App Password is used (not regular password)
- Check API console logs for email errors
- Email failures won't prevent order creation

## What Changed

### New Pages Created:
1. `/checkout` - Complete checkout form with guest support
2. `/order-confirmation/:orderNumber` - Order success page
3. `/admin/orders` - Admin order management panel

### Backend Updates:
- Order model supports guest orders
- Email service sends notifications
- Fixed 3 JOD shipping
- No tax calculation
- Simplified order statuses

### Frontend Updates:
- Guest checkout integration
- CliQ payment info display
- Order confirmation flow
- Admin order management with status updates
- Complete bilingual support (EN/AR)

