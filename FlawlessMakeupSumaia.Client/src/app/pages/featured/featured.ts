import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/product.model';
import { CartConfirmationComponent, CartConfirmationData } from '../../components/cart-confirmation/cart-confirmation';

@Component({
  selector: 'app-featured',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule, CartConfirmationComponent],
  templateUrl: './featured.html',
  styleUrl: './featured.scss'
})
export class FeaturedComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  products: Product[] = [];
  isLoading = true;
  isAddingToCart: { [productId: number]: boolean } = {};
  currentLang = 'en';
  
  showCartConfirmation = false;
  cartConfirmationData: CartConfirmationData | null = null;

  constructor(
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService,
    private router: Router,
    private translate: TranslateService
  ) { 
    this.currentLang = this.translate.currentLang || this.translate.defaultLang || 'en';
    
    this.translate.onLangChange.pipe(takeUntil(this.destroy$)).subscribe((event) => {
      this.currentLang = event.lang;
    });
  }

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
}