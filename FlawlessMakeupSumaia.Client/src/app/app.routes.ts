import { Routes } from '@angular/router';

export const routes: Routes = [
    { path: '', loadComponent: () => import('./pages/home/home').then(m => m.HomeComponent) },
    { path: 'login', loadComponent: () => import('./pages/login/login').then(m => m.LoginComponent) },
    { path: 'products', loadComponent: () => import('./pages/products/products').then(m => m.ProductsComponent) },
    { path: 'product/:id', loadComponent: () => import('./pages/product-detail/product-detail').then(m => m.ProductDetailComponent) },
    { path: 'category/:id', loadComponent: () => import('./pages/category-products/category-products').then(m => m.CategoryProductsComponent) },
    { path: 'featured', loadComponent: () => import('./pages/featured/featured').then(m => m.FeaturedComponent) },
    { path: 'sale', loadComponent: () => import('./pages/sale/sale').then(m => m.SaleComponent) },
    { path: 'cart', loadComponent: () => import('./pages/cart/cart').then(m => m.CartComponent) },

    // Admin Routes
    { path: 'admin', loadComponent: () => import('./admin/admin-dashboard/admin-dashboard').then(m => m.AdminDashboardComponent) },
    { path: 'admin/dashboard', loadComponent: () => import('./admin/admin-dashboard/admin-dashboard').then(m => m.AdminDashboardComponent) },
    { path: 'admin/products', loadComponent: () => import('./admin/product-management/product-management').then(m => m.ProductManagementComponent) },
    { path: 'admin/categories', loadComponent: () => import('./admin/category-management/category-management').then(m => m.CategoryManagementComponent) },

    { path: '**', redirectTo: '' }
];
