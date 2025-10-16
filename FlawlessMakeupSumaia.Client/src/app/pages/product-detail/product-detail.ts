import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, ActivatedRoute } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule } from '@ngx-translate/core';

import { ProductService } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { Product } from '../../models/product.model';
import { CartConfirmationComponent, CartConfirmationData } from '../../components/cart-confirmation/cart-confirmation';

@Component({
  selector: 'app-product-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule, TranslateModule, CartConfirmationComponent],
  templateUrl: './product-detail.html',
  styleUrl: './product-detail.scss'
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  product: Product | null = null;
  isLoading = true;
  quantity = 1;
  isAddingToCart = false;
  selectedShadeId: number | null = null;
  
  showCartConfirmation = false;
  cartConfirmationData: CartConfirmationData | null = null;

  constructor(
    private route: ActivatedRoute,
    private productService: ProductService,
    private cartService: CartService,
    private authService: AuthService
  ) { }

  ngOnInit(): void {
    this.route.params.pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const productId = Number(params['id']);
        if (productId) {
          this.loadProduct(productId);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProduct(id: number): void {
    this.isLoading = true;

    this.productService.getProduct(id)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (product) => {
          this.product = product;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading product:', error);
          this.isLoading = false;
        }
      });
  }

  increaseQuantity(): void {
    if (this.product && this.quantity < this.product.stockQuantity) {
      this.quantity++;
    }
  }

  decreaseQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  addToCart(): void {
    if (!this.product) return;

    // Validate shade selection if product has shades
    if (this.hasShadeOptions() && !this.selectedShadeId) {
      return; // Button is already disabled, but extra safety check
    }

    // CartService now handles both guest and authenticated users
    this.isAddingToCart = true;

    this.cartService.addToCart({
      productId: this.product.id,
      quantity: this.quantity,
      productShadeId: this.selectedShadeId || undefined
    }).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: () => {
          this.isAddingToCart = false;
          
          let cartCount = 0;
          this.cartService.cartItemCount$.pipe(takeUntil(this.destroy$)).subscribe(count => {
            cartCount = count;
          });
          
          this.cartConfirmationData = {
            productName: this.product!.name,
            productImage: this.product!.imageUrl,
            cartItemCount: cartCount
          };
          this.showCartConfirmation = true;
          
          // Reset selected shade after adding to cart
          this.selectedShadeId = null;
        },
        error: (error) => {
          console.error('Error adding to cart:', error);
          this.isAddingToCart = false;
        }
      });
  }

  onCartConfirmationClosed(): void {
    this.showCartConfirmation = false;
    this.cartConfirmationData = null;
  }

  getAvailableShades() {
    if (!this.product?.productShades) return [];
    return this.product.productShades.filter(s => s.isActive);
  }

  selectShade(shadeId: number): void {
    this.selectedShadeId = shadeId;
  }

  hasShadeOptions(): boolean {
    return this.getAvailableShades().length > 0;
  }

  getSelectedShade() {
    if (!this.selectedShadeId || !this.product) return null;
    return this.product.productShades.find(s => s.id === this.selectedShadeId);
  }
}