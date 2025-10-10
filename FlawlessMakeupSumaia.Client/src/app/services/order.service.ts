import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { Order, CreateOrder, OrderStatus } from '../models/order.model';

@Injectable({
    providedIn: 'root'
})
export class OrderService {
    constructor(private apiService: ApiService) { }

    getOrders(): Observable<Order[]> {
        return this.apiService.get<Order[]>('api/orders');
    }

    getOrder(id: number): Observable<Order> {
        return this.apiService.get<Order>(`api/orders/${id}`);
    }

    getOrderByNumber(orderNumber: string): Observable<Order> {
        return this.apiService.get<Order>(`api/orders/by-number/${orderNumber}`);
    }

    createOrderFromCart(order: CreateOrder): Observable<Order> {
        return this.apiService.post<Order>('api/orders/create-from-cart', order);
    }

    updateOrderStatus(id: number, status: OrderStatus): Observable<Order> {
        return this.apiService.put<Order>(`api/orders/${id}/status`, { status });
    }
}
