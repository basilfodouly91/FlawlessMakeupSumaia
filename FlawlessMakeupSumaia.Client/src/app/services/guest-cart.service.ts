import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface GuestCartItem {
  productId: number;
  productName: string;
  productImageUrl: string;
  productBrand: string;
  quantity: number;
  price: number;
  productShadeId?: number;
  productShadeName?: string;
  stockQuantity: number;
}

export interface GuestCart {
  items: GuestCartItem[];
  totalItems: number;
  totalAmount: number;
}

@Injectable({
  providedIn: 'root'
})
export class GuestCartService {
  private readonly CART_STORAGE_KEY = 'guest_cart';
  private cartSubject = new BehaviorSubject<GuestCart>(this.loadCart());
  public cart$ = this.cartSubject.asObservable();

  private cartItemCountSubject = new BehaviorSubject<number>(0);
  public cartItemCount$ = this.cartItemCountSubject.asObservable();

  constructor() {
    const cart = this.loadCart();
    this.cartItemCountSubject.next(cart.totalItems);
  }

  private loadCart(): GuestCart {
    try {
      const cartData = localStorage.getItem(this.CART_STORAGE_KEY);
      if (cartData) {
        const cart = JSON.parse(cartData) as GuestCart;
        return cart;
      }
    } catch (error) {
      console.error('Error loading guest cart:', error);
    }
    return { items: [], totalItems: 0, totalAmount: 0 };
  }

  private saveCart(cart: GuestCart): void {
    try {
      localStorage.setItem(this.CART_STORAGE_KEY, JSON.stringify(cart));
      this.cartSubject.next(cart);
      this.cartItemCountSubject.next(cart.totalItems);
    } catch (error) {
      console.error('Error saving guest cart:', error);
    }
  }

  private calculateTotals(cart: GuestCart): void {
    cart.totalItems = cart.items.reduce((sum, item) => sum + item.quantity, 0);
    cart.totalAmount = cart.items.reduce((sum, item) => sum + (item.price * item.quantity), 0);
  }

  getCart(): GuestCart {
    return this.loadCart();
  }

  addToCart(item: GuestCartItem): void {
    const cart = this.loadCart();
    // For products with shades, treat each shade as a separate item
    const existingItem = cart.items.find(i => 
      i.productId === item.productId && 
      i.productShadeId === item.productShadeId
    );

    if (existingItem) {
      existingItem.quantity += item.quantity;
    } else {
      cart.items.push(item);
    }

    this.calculateTotals(cart);
    this.saveCart(cart);
  }

  updateQuantity(productId: number, quantity: number): void {
    const cart = this.loadCart();
    const item = cart.items.find(i => i.productId === productId);

    if (item) {
      if (quantity <= 0) {
        cart.items = cart.items.filter(i => i.productId !== productId);
      } else {
        item.quantity = quantity;
      }
      this.calculateTotals(cart);
      this.saveCart(cart);
    }
  }

  removeFromCart(productId: number): void {
    const cart = this.loadCart();
    cart.items = cart.items.filter(i => i.productId !== productId);
    this.calculateTotals(cart);
    this.saveCart(cart);
  }

  clearCart(): void {
    const emptyCart: GuestCart = { items: [], totalItems: 0, totalAmount: 0 };
    this.saveCart(emptyCart);
  }

  getItemCount(): number {
    const cart = this.loadCart();
    return cart.totalItems;
  }

  // Convert guest cart to user cart when user logs in
  transferToUserCart(): GuestCartItem[] {
    const cart = this.loadCart();
    return cart.items;
  }
}

