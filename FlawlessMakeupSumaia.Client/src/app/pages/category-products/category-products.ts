import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, forkJoin } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/product.model';
import { Category } from '../../models/category.model';
import { CartConfirmationComponent, CartConfirmationData } from '../../components/cart-confirmation/cart-confirmation';

@Component({
  selector: 'app-category-products',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, TranslateModule, CartConfirmationComponent],
  templateUrl: './category-products.html',
  styleUrl: './category-products.scss'
})
export class CategoryProductsComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  category: Category | null = null;
  products: Product[] = [];
  isLoading = true;
  isAddingToCart: { [productId: number]: boolean } = {};
  currentLang = 'en';
  
  showCartConfirmation = false;
  cartConfirmationData: CartConfirmationData | null = null;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private productService: ProductService,
    private categoryService: CategoryService,
    private cartService: CartService,
    private authService: AuthService,
    private translate: TranslateService
  ) {
    this.currentLang = this.translate.currentLang || this.translate.defaultLang || 'en';
    
    // Subscribe to language changes
    this.translate.onLangChange.pipe(takeUntil(this.destroy$)).subscribe((event) => {
      this.currentLang = event.lang;
    });
  }

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

  addToCart(product: Product, event?: Event): void {
    if (event) {
      event.stopPropagation();
    }

    // Check if product has shades - if so, redirect to product details
    if (product.productShades && product.productShades.length > 0) {
      this.router.navigate(['/product', product.id]);
      return;
    }

    // CartService now handles both guest and authenticated users
    this.isAddingToCart[product.id] = true;

    this.cartService.addToCart({
      productId: product.id,
      quantity: 1
    }).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isAddingToCart[product.id] = false;
          
          let cartCount = 0;
          this.cartService.cartItemCount$.pipe(takeUntil(this.destroy$)).subscribe(count => {
            cartCount = count;
          });
          
          this.cartConfirmationData = {
            productName: product.name,
            productImage: product.imageUrl,
            cartItemCount: cartCount
          };
          this.showCartConfirmation = true;
        },
        error: (error) => {
          console.error('Error adding to cart:', error);
          this.isAddingToCart[product.id] = false;
          const errorMsg = this.currentLang === 'ar'
            ? 'حدث خطأ أثناء إضافة المنتج'
            : 'Error adding product';
          alert(errorMsg);
        }
      });
  }

  onCartConfirmationClosed(): void {
    this.showCartConfirmation = false;
    this.cartConfirmationData = null;
  }

  getCategoryName(category: Category): string {
    return this.currentLang === 'ar' ? category.nameAr : category.nameEn;
  }
}