import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil, forkJoin } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { ProductService } from '../../services/product.service';
import { CategoryService } from '../../services/category.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { Product } from '../../models/product.model';
import { Category } from '../../models/category.model';
import { CartConfirmationComponent, CartConfirmationData } from '../../components/cart-confirmation/cart-confirmation';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, TranslateModule, CartConfirmationComponent],
  templateUrl: './home.html',
  styleUrl: './home.scss'
})
export class HomeComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  categories: Category[] = [];
  featuredProducts: Product[] = [];
  saleProducts: Product[] = [];
  newsletterEmail = '';
  currentLang = 'en';

  // Loading states
  isLoadingCategories = true;
  isLoadingFeatured = true;
  isLoadingSale = true;
  isAddingToCart: { [productId: number]: boolean } = {};

  // Cart confirmation
  showCartConfirmation = false;
  cartConfirmationData: CartConfirmationData | null = null;

  constructor(
    private productService: ProductService,
    private categoryService: CategoryService,
    private cartService: CartService,
    private authService: AuthService,
    private notificationService: NotificationService,
    private translate: TranslateService,
    private router: Router
  ) { 
    this.currentLang = this.translate.currentLang || this.translate.defaultLang || 'en';
    
    // Subscribe to language changes
    this.translate.onLangChange.pipe(takeUntil(this.destroy$)).subscribe((event) => {
      this.currentLang = event.lang;
    });
  }

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

  addToCart(product: Product, event?: Event): void {
    // Prevent navigation when clicking the add to cart button
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
          
          // Get current cart count
          let cartCount = 0;
          this.cartService.cartItemCount$.pipe(takeUntil(this.destroy$)).subscribe(count => {
            cartCount = count;
          });
          
          // Show confirmation dialog
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
          this.notificationService.error(errorMsg);
        }
      });
  }

  onCartConfirmationClosed(): void {
    this.showCartConfirmation = false;
    this.cartConfirmationData = null;
  }

  onNewsletterSubmit(): void {
    if (this.newsletterEmail.trim()) {
      // Here you would typically call a newsletter service
      console.log('Newsletter subscription for:', this.newsletterEmail);
      const successMsg = this.currentLang === 'ar'
        ? 'شكراً لك على الاشتراك في نشرتنا الإخبارية!'
        : 'Thank you for subscribing to our newsletter!';
      this.notificationService.success(successMsg);
      this.newsletterEmail = '';
    }
  }

  getCategoryName(category: Category): string {
    return this.currentLang === 'ar' ? category.nameAr : category.nameEn;
  }
}