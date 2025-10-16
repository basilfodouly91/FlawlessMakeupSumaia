import { Injectable } from '@angular/core';
import { Observable, BehaviorSubject, of, from } from 'rxjs';
import { tap, switchMap, catchError } from 'rxjs/operators';
import { ApiService } from './api.service';
import { AuthService } from './auth.service';
import { GuestCartService, GuestCartItem } from './guest-cart.service';
import { ProductService } from './product.service';
import { Cart, AddToCart, UpdateCartItem } from '../models/cart.model';

@Injectable({
    providedIn: 'root'
})
export class CartService {
    private cartSubject = new BehaviorSubject<Cart | null>(null);
    public cart$ = this.cartSubject.asObservable();

    private cartItemCountSubject = new BehaviorSubject<number>(0);
    public cartItemCount$ = this.cartItemCountSubject.asObservable();

    constructor(
        private apiService: ApiService,
        private authService: AuthService,
        private guestCartService: GuestCartService,
        private productService: ProductService
    ) {
        this.loadCart();
        
        // Subscribe to guest cart count changes
        this.guestCartService.cartItemCount$.subscribe(count => {
            if (!this.authService.isLoggedIn()) {
                this.cartItemCountSubject.next(count);
            }
        });
        
        // Subscribe to auth changes
        this.authService.isLoggedIn$.subscribe(isLoggedIn => {
            if (isLoggedIn) {
                // Transfer guest cart to user cart when user logs in
                this.transferGuestCartToUser();
            } else {
                // Load guest cart count
                const guestCart = this.guestCartService.getCart();
                this.cartItemCountSubject.next(guestCart.totalItems);
            }
        });
    }

    getCart(): Observable<Cart> {
        return this.apiService.get<Cart>('api/cart').pipe(
            tap(cart => {
                this.cartSubject.next(cart);
                this.cartItemCountSubject.next(cart.totalItems);
            })
        );
    }

    addToCart(item: AddToCart): Observable<any> {
        // If user is not logged in, use guest cart
        if (!this.authService.isLoggedIn()) {
            return from(this.addToGuestCart(item));
        }

        // If user is logged in, use API
        return this.apiService.post<Cart>('api/cart/add', item).pipe(
            tap(cart => {
                this.cartSubject.next(cart);
                this.cartItemCountSubject.next(cart.totalItems);
            }),
            catchError(error => {
                console.error('Error adding to cart via API:', error);
                throw error;
            })
        );
    }

    private async addToGuestCart(item: AddToCart): Promise<void> {
        try {
            // Load product details
            const product = await this.productService.getProduct(item.productId).toPromise();
            if (!product) {
                throw new Error('Product not found');
            }

            // Find the selected shade details if a shade ID is provided
            const selectedShade = item.productShadeId 
                ? product.productShades.find(s => s.id === item.productShadeId)
                : undefined;

            const guestItem: GuestCartItem = {
                productId: product.id,
                productName: product.name,
                productImageUrl: product.imageUrl,
                productBrand: product.brand || '',
                quantity: item.quantity,
                price: product.currentPrice,
                productShadeId: item.productShadeId,
                productShadeName: selectedShade?.name,
                stockQuantity: selectedShade?.stockQuantity ?? product.stockQuantity
            };

            this.guestCartService.addToCart(guestItem);
        } catch (error) {
            console.error('Error adding to guest cart:', error);
            throw error;
        }
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
        } else {
            // Load guest cart count
            const guestCart = this.guestCartService.getCart();
            this.cartItemCountSubject.next(guestCart.totalItems);
        }
    }

    refreshCart(): void {
        this.loadCart();
    }

    private transferGuestCartToUser(): void {
        const guestItems = this.guestCartService.transferToUserCart();
        
        if (guestItems.length > 0) {
            // Transfer each item to user cart
            guestItems.forEach(item => {
                this.addToCart({
                    productId: item.productId,
                    quantity: item.quantity,
                    productShadeId: item.productShadeId
                }).subscribe({
                    next: () => console.log(`Transferred ${item.productName} to user cart`),
                    error: (error) => console.error('Error transferring item:', error)
                });
            });

            // Clear guest cart after transfer
            this.guestCartService.clearCart();
        } else {
            // No guest items, just load user cart
            this.loadCart();
        }
    }
}
