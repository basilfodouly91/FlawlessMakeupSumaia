import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Product, CreateProduct } from '../models/product.model';

@Injectable({
    providedIn: 'root'
})
export class ProductService {
    constructor(private apiService: ApiService) { }

    getAllProducts(): Observable<Product[]> {
        return this.apiService.get<Product[]>('api/products');
    }

    getFeaturedProducts(): Observable<Product[]> {
        return this.apiService.get<Product[]>('api/products/featured');
    }

    getProductsOnSale(): Observable<Product[]> {
        return this.apiService.get<Product[]>('api/products/on-sale');
    }

    getProductsByCategory(categoryId: number): Observable<Product[]> {
        return this.apiService.get<Product[]>(`api/products/category/${categoryId}`);
    }

    getProduct(id: number): Observable<Product> {
        return this.apiService.get<Product>(`api/products/${id}`);
    }

    searchProducts(searchTerm: string): Observable<Product[]> {
        return this.apiService.get<Product[]>(`api/products/search?q=${encodeURIComponent(searchTerm)}`);
    }

    createProduct(product: CreateProduct): Observable<Product> {
        return this.apiService.post<Product>('api/products', product);
    }

    updateProduct(id: number, product: any): Observable<Product> {
        return this.apiService.put<Product>(`api/products/${id}`, product);
    }

    deleteProduct(id: number): Observable<void> {
        return this.apiService.delete<void>(`api/products/${id}`);
    }
}
