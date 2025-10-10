import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, forkJoin } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/product.model';
import { Category } from '../../models/category.model';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, TranslateModule],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class HomeComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  categories: Category[] = [];
  featuredProducts: Product[] = [];
  saleProducts: Product[] = [];
  newsletterEmail = '';

  // Loading states
  isLoadingCategories = true;
  isLoadingFeatured = true;
  isLoadingSale = true;
  isAddingToCart: { [productId: number]: boolean } = {};

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private cartService: CartService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.loadData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadData(): void {
    // Load all data in parallel
    forkJoin({
      categories: this.categoryService.getAllCategories(),
      featured: this.productService.getFeaturedProducts(),
      sale: this.productService.getProductsOnSale()
    }).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.categories = data.categories;
          this.featuredProducts = data.featured.slice(0, 8); // Show first 8
          this.saleProducts = data.sale.slice(0, 8); // Show first 8

          this.isLoadingCategories = false;
          this.isLoadingFeatured = false;
          this.isLoadingSale = false;
        },
        error: (error) => {
          console.error('Error loading data:', error);
          this.isLoadingCategories = false;
          this.isLoadingFeatured = false;
          this.isLoadingSale = false;
        }
      });
  }

  addToCart(product: Product): void {
    if (!this.authService.isLoggedIn()) {
      // Redirect to login if not authenticated
      // You might want to show a modal or redirect
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
          // You might want to show a success message
        },
        error: (error) => {
          console.error('Error adding to cart:', error);
          this.isAddingToCart[product.id] = false;
          // You might want to show an error message
        }
      });
  }

  onNewsletterSubmit(): void {
    if (this.newsletterEmail.trim()) {
      // Here you would typically call a newsletter service
      console.log('Newsletter subscription for:', this.newsletterEmail);
      alert('Thank you for subscribing to our newsletter!');
      this.newsletterEmail = '';
    }
  }
}