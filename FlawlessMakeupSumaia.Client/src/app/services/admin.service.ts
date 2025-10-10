import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import {
    AdminDashboard,
    AdminProduct,
    AdminCategory,
    BulkUpdate,
    SaleToggle,
    StockUpdate,
    ProductAnalytics
} from '../models/admin.model';
import { Product, CreateProduct } from '../models/product.model';
import { Category } from '../models/category.model';

@Injectable({
    providedIn: 'root'
})
export class AdminService {
    constructor(private apiService: ApiService) { }

    // Dashboard
    getDashboard(): Observable<AdminDashboard> {
        return this.apiService.get<AdminDashboard>('api/admin/dashboard');
    }

    // Product Management
    getAllProductsForAdmin(): Observable<AdminProduct[]> {
        return this.apiService.get<AdminProduct[]>('api/admin/products');
    }

    bulkUpdateProducts(bulkUpdate: BulkUpdate): Observable<any> {
        return this.apiService.post('api/admin/products/bulk-update', bulkUpdate);
    }

    toggleProductSale(id: number, saleData: SaleToggle): Observable<Product> {
        return this.apiService.post<Product>(`api/admin/products/${id}/toggle-sale`, saleData);
    }

    toggleProductFeatured(id: number): Observable<Product> {
        return this.apiService.post<Product>(`api/admin/products/${id}/toggle-featured`, {});
    }

    toggleProductActive(id: number): Observable<Product> {
        return this.apiService.post<Product>(`api/admin/products/${id}/toggle-active`, {});
    }

    updateStock(id: number, stockUpdate: StockUpdate): Observable<Product> {
        return this.apiService.put<Product>(`api/admin/products/${id}/stock`, stockUpdate);
    }

    // Category Management
    getAllCategoriesForAdmin(): Observable<AdminCategory[]> {
        return this.apiService.get<AdminCategory[]>('api/admin/categories');
    }

    toggleCategoryActive(id: number): Observable<Category> {
        return this.apiService.post<Category>(`api/admin/categories/${id}/toggle-active`, {});
    }

    // Analytics
    getProductAnalytics(): Observable<ProductAnalytics> {
        return this.apiService.get<ProductAnalytics>('api/admin/analytics/products');
    }

    // Regular CRUD operations (using existing services)
    createProduct(product: CreateProduct): Observable<Product> {
        return this.apiService.post<Product>('api/products', product);
    }

    updateProduct(id: number, product: any): Observable<Product> {
        return this.apiService.put<Product>(`api/products/${id}`, product);
    }

    deleteProduct(id: number): Observable<void> {
        return this.apiService.delete<void>(`api/products/${id}`);
    }

    createCategory(category: any): Observable<Category> {
        return this.apiService.post<Category>('api/categories', category);
    }

    updateCategory(id: number, category: any): Observable<Category> {
        return this.apiService.put<Category>(`api/categories/${id}`, category);
    }

    deleteCategory(id: number): Observable<void> {
        return this.apiService.delete<void>(`api/categories/${id}`);
    }
}
