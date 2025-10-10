import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/product.model';

@Component({
  selector: 'app-featured',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  templateUrl: './featured.html',
  styleUrl: './featured.scss'
})
export class FeaturedComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  products: Product[] = [];
  isLoading = true;
  isAddingToCart: { [productId: number]: boolean } = {};

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadFeaturedProducts();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadFeaturedProducts(): void {
    this.isLoading = true;

    this.productService.getFeaturedProducts()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (products) => {
          this.products = products;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading featured products:', error);
          this.isLoading = false;
        }
      });
  }

  addToCart(product: Product): void {
    if (!this.authService.isLoggedIn()) {
      alert('Please log in to add items to cart');
      return;
    }

    this.isAddingToCart[product.id] = true;

    this.cartService.addToCart({
      productId: product.id,
      quantity: 1
    }).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isAddingToCart[product.id] = false;
          alert(`${product.name} added to cart!`);
        },
        error: (error) => {
          console.error('Error adding to cart:', error);
          this.isAddingToCart[product.id] = false;
          alert('Error adding to cart. Please try again.');
        }
      });
  }
}