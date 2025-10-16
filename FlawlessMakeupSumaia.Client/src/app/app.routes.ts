import { Routes } from '@angular/router';
import { adminGuard } from './guards/admin.guard';

export const routes: Routes = [
    { path: '', loadComponent: () => import('./pages/home/home').then(m => m.HomeComponent) },
    { path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.LoginComponent) },
    { path: 'products', loadComponent: () => import('./pages/products/products').then(m => m.ProductsComponent) },
    { path: 'product/:id', loadComponent: () => import('./pages/product-detail/product-detail').then(m => m.ProductDetailComponent) },
    { path: 'category/:id', loadComponent: () => import('./pages/category-products/category-products').then(m => m.CategoryProductsComponent) },
    { path: 'featured', loadComponent: () => import('./pages/featured/featured').then(m => m.FeaturedComponent) },
    { path: 'sale', loadComponent: () => import('./pages/sale/sale').then(m => m.SaleComponent) },
    { path: 'cart', loadComponent: () => import('./pages/cart/cart').then(m => m.CartComponent) },
    { path: 'checkout', loadComponent: () => import('./pages/checkout/checkout').then(m => m.CheckoutComponent) },
    { path: 'order-confirmation/:orderNumber', loadComponent: () => import('./pages/order-confirmation/order-confirmation').then(m => m.OrderConfirmationComponent) },

    // Admin Routes - Protected by adminGuard
    { path: 'admin', loadComponent: () => import('./admin/admin-dashboard/admin-dashboard').then(m => m.AdminDashboardComponent), canActivate: [adminGuard] },
    { path: 'admin/dashboard', loadComponent: () => import('./admin/admin-dashboard/admin-dashboard').then(m => m.AdminDashboardComponent), canActivate: [adminGuard] },
    { path: 'admin/products', loadComponent: () => import('./admin/product-management/product-management').then(m => m.ProductManagementComponent), canActivate: [adminGuard] },
    { path: 'admin/categories', loadComponent: () => import('./admin/category-management/category-management').then(m => m.CategoryManagementComponent), canActivate: [adminGuard] },
    { path: 'admin/orders', loadComponent: () => import('./admin/order-management/order-management').then(m => m.OrderManagementComponent), canActivate: [adminGuard] },

    { path: '**', redirectTo: '' }
];
