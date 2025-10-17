import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { GuestCartService, GuestCart } from '../../services/guest-cart.service';
import { Cart, CartItem } from '../../models/cart.model';

@Component({
  selector: 'app-cart',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  templateUrl: './cart.html',
  styleUrl: './cart.scss'
})
export class CartComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  cart: Cart | null = null;
  guestCart: GuestCart | null = null;
  isGuest: boolean = true;

  constructor(
    private cartService: CartService,
    private authService: AuthService,
    private guestCartService: GuestCartService
  ) { }

  ngOnInit(): void {
    this.isGuest = !this.authService.isLoggedIn();
    this.loadCart();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCart(): void {
    if (this.isGuest) {
      // Load guest cart from localStorage
      this.guestCart = this.guestCartService.getCart();
    } else {
      // Load authenticated user cart from API
      this.cartService.getCart()
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (cart) => this.cart = cart,
          error: (error) => console.error('Error loading cart:', error)
        });
    }
  }

  updateQuantity(item: CartItem | any, newQuantity: number): void {
    if (newQuantity < 1) {
      this.removeItem(item);
      return;
    }

    if (this.isGuest) {
      // Update guest cart
      this.guestCartService.updateQuantity(item.productId, newQuantity);
      this.guestCart = this.guestCartService.getCart();
    } else {
      // Update authenticated cart
      this.cartService.updateCartItem({
        productId: item.productId,
        quantity: newQuantity
      }).pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (cart) => this.cart = cart,
          error: (error) => console.error('Error updating cart:', error)
        });
    }
  }

  onQuantityChange(item: CartItem | any, event: any): void {
    const newQuantity = parseInt(event.target.value);
    if (!isNaN(newQuantity) && newQuantity > 0) {
      this.updateQuantity(item, newQuantity);
    }
  }

  removeItem(item: CartItem | any): void {
    if (this.isGuest) {
      // Remove from guest cart
      this.guestCartService.removeFromCart(item.productId);
      this.guestCart = this.guestCartService.getCart();
    } else {
      // Remove from authenticated cart
      this.cartService.removeFromCart(item.productId)
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (cart) => this.cart = cart,
          error: (error) => console.error('Error removing item:', error)
        });
    }
  }

  getTotal(): number {
    if (this.isGuest && this.guestCart) {
      const shipping = 3;
      return this.guestCart.totalAmount + shipping;
    }
    if (!this.isGuest && this.cart) {
      const shipping = 3;
      return this.cart.totalAmount + shipping;
    }
    return 0;
  }

  getTotalItems(): number {
    if (this.isGuest && this.guestCart) {
      return this.guestCart.totalItems;
    }
    if (!this.isGuest && this.cart) {
      return this.cart.totalItems;
    }
    return 0;
  }

  getCartItems(): any[] {
    if (this.isGuest && this.guestCart) {
      return this.guestCart.items;
    }
    if (!this.isGuest && this.cart) {
      return this.cart.cartItems;
    }
    return [];
  }
}