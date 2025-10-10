import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
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
  selector: 'app-category-products',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, TranslateModule],
  templateUrl: './category-products.html',
  styleUrl: './category-products.scss'
})
export class CategoryProductsComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  category: Category | null = null;
  products: Product[] = [];
  isLoading = true;
  isAddingToCart: { [productId: number]: boolean } = {};

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private categoryService: CategoryService,
    private cartService: CartService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.route.params.pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const categoryId = Number(params['id']);
        if (categoryId) {
          this.loadCategoryData(categoryId);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCategoryData(categoryId: number): void {
    this.isLoading = true;

    forkJoin({
      category: this.categoryService.getCategory(categoryId),
      products: this.productService.getProductsByCategory(categoryId)
    }).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.category = data.category;
          this.products = data.products;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading category data:', error);
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