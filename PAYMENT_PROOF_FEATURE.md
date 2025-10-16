# Payment Proof Upload Feature

## Overview
Customers can now upload a screenshot of their CliQ payment transfer when placing an order. This helps verify payments and streamlines order processing.

## How It Works

### For Customers (Checkout Page)

1. **Go to Checkout:** http://localhost:4200/checkout
2. **Fill in shipping information**
3. **In the Payment Section:**
   - See the CliQ username: **BASILFODOULY**
   - Click "Copy CliQ" to copy to clipboard
   - Make the CliQ payment transfer
   - **Upload payment proof screenshot** using the file upload field
   - Preview the uploaded image before submitting
   - Remove and re-upload if needed

4. **Place Order**
   - The payment proof image is saved with the order
   - Image is stored as base64 in the database

### For Admin (Order Management)

1. **View Orders:** http://localhost:4200/admin/orders
2. **Click "View Details"** on any order
3. **See Payment Proof:**
   - If customer uploaded a payment proof, it displays in the order details modal
   - Under "Payment Proof" section
   - Click image to view full size

### Email Notifications

When an order is placed with a payment proof:
- The admin email notification includes the payment proof image
- Displayed in the email under "Payment Information" section

## Technical Details

### Database Changes
- **Column Added:** `PaymentProofImageUrl` (NVARCHAR(MAX), NULL)
- **Location:** Orders table
- **Storage:** Base64 encoded image data

### Files Modified

**Backend:**
- `FlawlessMakeupSumaia.API/Models/Order.cs` - Added PaymentProofImageUrl property
- `FlawlessMakeupSumaia.API/DTOs/OrderDto.cs` - Added to DTOs
- `FlawlessMakeupSumaia.API/Services/MappingService.cs` - Updated mappings
- `FlawlessMakeupSumaia.API/Services/EmailService.cs` - Include image in email
- `FlawlessMakeupSumaia.API/Controllers/OrdersController.cs` - Process uploaded image

**Frontend:**
- `FlawlessMakeupSumaia.Client/src/app/models/order.model.ts` - Added property
- `FlawlessMakeupSumaia.Client/src/app/pages/checkout/checkout.ts` - Upload handling
- `FlawlessMakeupSumaia.Client/src/app/pages/checkout/checkout.html` - UI for upload
- `FlawlessMakeupSumaia.Client/src/app/pages/checkout/checkout.scss` - Styling
- `FlawlessMakeupSumaia.Client/src/app/admin/order-management/order-management.html` - Display in admin
- `FlawlessMakeupSumaia.Client/src/app/admin/order-management/order-management.scss` - Styling
- Translations (en.json, ar.json)

### Features

✅ **File Upload:**
- File type validation (images only)
- File size validation (max 5MB)
- Base64 conversion for storage
- Preview before submission
- Remove and re-upload option

✅ **Display:**
- Image preview on checkout page
- Image display in admin order details
- Image included in email notifications

✅ **Bilingual:**
- Full English and Arabic support
- Translated labels and hints

## Testing

1. **Test Upload:**
   - Go to checkout with items in cart
   - Upload a screenshot (PNG/JPG)
   - See preview appear
   - Submit order
   - Check confirmation page

2. **Test Admin View:**
   - Login as admin
   - Go to /admin/orders
   - Click "View Details" on the order
   - See the uploaded payment proof image

3. **Test Email (if configured):**
   - Place an order with payment proof
   - Check admin email
   - Payment proof image should be visible in email

## Usage Notes

- **Optional Field:** Customers can skip uploading if they prefer
- **Max Size:** 5MB per image
- **Formats:** JPG, PNG, GIF, WebP (any image format)
- **Storage:** Base64 in database (for simplicity)
- **Alternative:** For production, consider storing images in cloud storage (Azure Blob, AWS S3) for better performance

## Quick Start

1. **Restart API Server** (to apply changes)
2. **Refresh checkout page** in browser
3. **Try uploading** a payment screenshot
4. **View in admin panel** after placing order

The feature is ready to use!

