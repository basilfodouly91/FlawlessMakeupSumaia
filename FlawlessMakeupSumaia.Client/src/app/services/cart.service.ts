import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ApiService } from './api.service';
import { Cart, AddToCart, UpdateCartItem } from '../models/cart.model';

@Injectable({
    providedIn: 'root'
})
export class CartService {
    private cartSubject = new BehaviorSubject<Cart | null>(null);
    public cart$ = this.cartSubject.asObservable();

    private cartItemCountSubject = new BehaviorSubject<number>(0);
    public cartItemCount$ = this.cartItemCountSubject.asObservable();

    constructor(private apiService: ApiService) {
        this.loadCart();
    }

    getCart(): Observable<Cart> {
        return this.apiService.get<Cart>('api/cart').pipe(
            tap(cart => {
                this.cartSubject.next(cart);
                this.cartItemCountSubject.next(cart.totalItems);
            })
        );
    }

    addToCart(item: AddToCart): Observable<Cart> {
        return this.apiService.post<Cart>('api/cart/add', item).pipe(
            tap(cart => {
                this.cartSubject.next(cart);
                this.cartItemCountSubject.next(cart.totalItems);
            })
        );
    }

    updateCartItem(item: UpdateCartItem): Observable<Cart> {
        return this.apiService.put<Cart>('api/cart/update', item).pipe(
            tap(cart => {
                this.cartSubject.next(cart);
                this.cartItemCountSubject.next(cart.totalItems);
            })
        );
    }

    removeFromCart(productId: number): Observable<Cart> {
        return this.apiService.delete<Cart>(`api/cart/remove/${productId}`).pipe(
            tap(cart => {
                this.cartSubject.next(cart);
                this.cartItemCountSubject.next(cart.totalItems);
            })
        );
    }

    clearCart(): Observable<void> {
        return this.apiService.delete<void>('api/cart/clear').pipe(
            tap(() => {
                this.cartSubject.next(null);
                this.cartItemCountSubject.next(0);
            })
        );
    }

    getCartItemCount(): Observable<number> {
        return this.apiService.get<number>('api/cart/count').pipe(
            tap(count => this.cartItemCountSubject.next(count))
        );
    }

    private loadCart(): void {
        const token = localStorage.getItem('token');
        if (token) {
            this.getCart().subscribe();
            this.getCartItemCount().subscribe();
        }
    }

    refreshCart(): void {
        this.loadCart();
    }
}
