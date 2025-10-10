import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Category } from '../models/category.model';

@Injectable({
    providedIn: 'root'
})
export class CategoryService {
    constructor(private apiService: ApiService) { }

    getAllCategories(): Observable<Category[]> {
        return this.apiService.get<Category[]>('api/categories');
    }

    getCategory(id: number): Observable<Category> {
        return this.apiService.get<Category>(`api/categories/${id}`);
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
