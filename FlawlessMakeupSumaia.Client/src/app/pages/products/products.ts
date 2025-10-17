import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { NotificationService } from '../../services/notification.service';
import { Product } from '../../models/product.model';
import { CartConfirmationComponent, CartConfirmationData } from '../../components/cart-confirmation/cart-confirmation';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, CartConfirmationComponent],
  templateUrl: './products.html',
  styleUrl: './products.scss'
})
export class ProductsComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  products: Product[] = [];
  filteredProducts: Product[] = [];
  searchTerm: string = '';
  isLoading = true;
  isAddingToCart: { [productId: number]: boolean } = {};
  currentLang = 'en';

  // Cart confirmation
  showCartConfirmation = false;
  cartConfirmationData: CartConfirmationData | null = null;

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private notificationService: NotificationService,
    private route: ActivatedRoute,
    private router: Router,
    private translate: TranslateService
  ) { 
    this.currentLang = this.translate.currentLang || this.translate.defaultLang || 'en';
    
    // Subscribe to language changes
    this.translate.onLangChange.pipe(takeUntil(this.destroy$)).subscribe((event) => {
      this.currentLang = event.lang;
    });
  }

  ngOnInit(): void {
    // Check for search query parameter first
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(params => {
      if (params['search']) {
        this.searchTerm = params['search'];
      }
    });
    
    this.loadProducts();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProducts(): void {
    this.productService.getAllProducts()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (products) => {
          this.products = products;
          this.filteredProducts = products;
          this.isLoading = false;
          
          // Apply search if there's a search term from URL
          if (this.searchTerm) {
            this.filterProducts();
          }
        },
        error: (error) => {
          console.error('Error loading products:', error);
          this.isLoading = false;
        }
      });
  }

  filterProducts(): void {
    if (!this.searchTerm.trim()) {
      this.filteredProducts = this.products;
      return;
    }

    const searchLower = this.searchTerm.toLowerCase().trim();
    this.filteredProducts = this.products.filter(product =>
      product.name.toLowerCase().includes(searchLower) ||
      product.brand?.toLowerCase().includes(searchLower) ||
      product.description?.toLowerCase().includes(searchLower)
    );
  }

  viewProductDetails(product: Product): void {
    this.router.navigate(['/product', product.id]);
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
}