# Flawless Makeup Sumaia - Login Credentials

## 🔐 Admin Account

Use these credentials to access the admin dashboard:

- **Email**: `admin@flawlessmakeup.com`
- **Password**: `Admin@123`

### Admin Capabilities:
- ✅ Access admin dashboard
- ✅ Manage products (Create, Read, Update, Delete)
- ✅ Manage categories (Create, Read, Update, Delete)
- ✅ View all orders
- ✅ See analytics and reports

---

## 👤 Regular User Account

You can create a regular user account by:

1. Going to http://localhost:4200/login
2. Clicking "Sign Up" (or Arabic equivalent)
3. Filling in the registration form:
   - Email: your-email@example.com
   - Password: (must be strong, e.g., User@123)
   - Confirm Password: (same as above)
   - First Name: Your first name
   - Last Name: Your last name

### User Capabilities:
- ✅ Browse products and categories
- ✅ Add items to cart
- ✅ Place orders
- ✅ View personal order history
- ❌ Cannot access admin dashboard

---

## 🚀 How to Login

### As Admin:
1. Go to http://localhost:4200/login
2. Enter: `admin@flawlessmakeup.com`
3. Enter: `Admin@123`
4. Click "Sign In"
5. You'll see the "Admin" menu in the navigation bar

### As Regular User:
1. Register first (if you haven't)
2. Login with your email and password
3. You can shop and place orders
4. No admin menu will appear (by design)

---

## ⚠️ Important Notes

1. **Database Reset**: If you delete the database files (`flawlessmakeup.db`), the admin account will be recreated automatically when you restart the backend.

2. **Password Requirements**:
   - At least 6 characters (Identity default)
   - Recommended: Use special characters, numbers, uppercase and lowercase letters

3. **Role Assignment**:
   - All new registrations automatically get the "User" role
   - Only the admin account has the "Admin" role
   - Roles cannot be changed through the UI (by design, for security)

---

## 🔧 Troubleshooting

If you can't login:

1. **Verify servers are running**:
   - Backend: http://localhost:5001
   - Frontend: http://localhost:4200

2. **Check credentials carefully**:
   - Email is case-sensitive
   - Password is case-sensitive
   - Make sure there are no extra spaces

3. **Clear browser cache**:
   - Press `Ctrl+Shift+Delete`
   - Clear cached images and files
   - Refresh the page

4. **Check browser console**:
   - Press `F12` to open DevTools
   - Look for any error messages in the Console tab

---

**Last Updated**: October 12, 2025
**Database Version**: Fresh installation with corrected admin credentials






















