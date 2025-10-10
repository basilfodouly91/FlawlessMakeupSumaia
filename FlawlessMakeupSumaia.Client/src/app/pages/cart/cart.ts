import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

import { CartService } from '../../services/cart.service';
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

  constructor(private cartService: CartService) { }

  ngOnInit(): void {
    this.loadCart();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCart(): void {
    this.cartService.getCart()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (cart) => this.cart = cart,
        error: (error) => console.error('Error loading cart:', error)
      });
  }

  updateQuantity(item: CartItem, newQuantity: number): void {
    if (newQuantity < 1) {
      this.removeItem(item);
      return;
    }

    this.cartService.updateCartItem({
      productId: item.productId,
      quantity: newQuantity
    }).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (cart) => this.cart = cart,
        error: (error) => console.error('Error updating cart:', error)
      });
  }

  onQuantityChange(item: CartItem, event: any): void {
    const newQuantity = parseInt(event.target.value);
    if (!isNaN(newQuantity) && newQuantity > 0) {
      this.updateQuantity(item, newQuantity);
    }
  }

  removeItem(item: CartItem): void {
    this.cartService.removeFromCart(item.productId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (cart) => this.cart = cart,
        error: (error) => console.error('Error removing item:', error)
      });
  }

  getTotal(): number {
    if (!this.cart) return 0;
    const shipping = this.cart.totalAmount > 50 ? 0 : 5;
    return this.cart.totalAmount + shipping;
  }
}