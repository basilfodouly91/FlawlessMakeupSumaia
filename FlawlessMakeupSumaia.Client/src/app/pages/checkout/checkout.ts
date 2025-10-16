import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { CartService } from '../../services/cart.service';
import { OrderService } from '../../services/order.service';
import { AuthService } from '../../services/auth.service';
import { GuestCartService } from '../../services/guest-cart.service';
import { Cart } from '../../models/cart.model';
import { CreateOrder } from '../../models/order.model';

@Component({
  selector: 'app-checkout',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslateModule],
  templateUrl: './checkout.html',
  styleUrl: './checkout.scss'
})
export class CheckoutComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  cart: Cart | null = null;
  currentLang = 'en';
  isLoading = true;
  isPlacingOrder = false;
  isAuthenticated = false;
  cliqCopied = false;
  paymentProofPreview: string | null = null;

  // Form data
  orderForm: CreateOrder = {
    guestEmail: '',
    guestName: '',
    shippingFirstName: '',
    shippingLastName: '',
    shippingAddress: '',
    shippingAddress2: '',
    shippingCity: '',
    shippingState: '',
    shippingZipCode: '',
    shippingCountry: 'Jordan',
    shippingPhone: '',
    paymentMethod: 'Cash on Delivery',
    paymentProofImageUrl: '',
    notes: ''
  };

  constructor(
    private cartService: CartService,
    private orderService: OrderService,
    private authService: AuthService,
    private guestCartService: GuestCartService,
    private router: Router,
    private translate: TranslateService
  ) {
    this.currentLang = this.translate.currentLang || this.translate.defaultLang || 'en';
    
    this.translate.onLangChange.pipe(takeUntil(this.destroy$)).subscribe((event) => {
      this.currentLang = event.lang;
    });
  }

  ngOnInit(): void {
    this.isAuthenticated = this.authService.isLoggedIn();
    this.loadCart();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadCart(): void {
    if (this.isAuthenticated) {
      this.cartService.getCart()
        .pipe(takeUntil(this.destroy$))
        .subscribe({
          next: (cart) => {
            this.cart = cart;
            this.isLoading = false;
            
            if (!cart || cart.totalItems === 0) {
              this.router.navigate(['/cart']);
            }
          },
          error: (error) => {
            console.error('Error loading cart:', error);
            this.isLoading = false;
            this.router.navigate(['/cart']);
          }
        });
    } else {
      // Load guest cart
      const guestCart = this.guestCartService.getCart();
      if (guestCart.totalItems === 0) {
        this.router.navigate(['/cart']);
        return;
      }
      
      // Convert guest cart to Cart format for display
      this.cart = {
        id: 0,
        userId: '',
        cartItems: guestCart.items.map(item => ({
          id: 0,
          productId: item.productId,
          productName: item.productName,
          productImageUrl: item.productImageUrl,
          productBrand: item.productBrand,
          quantity: item.quantity,
          price: item.price,
          productShadeId: item.productShadeId,
          productShadeName: item.productShadeName,
          totalPrice: item.price * item.quantity,
          dateAdded: new Date(),
          isInStock: true,
          stockQuantity: item.stockQuantity
        })),
        dateCreated: new Date(),
        dateUpdated: new Date(),
        totalItems: guestCart.totalItems,
        totalAmount: guestCart.totalAmount
      };
      this.isLoading = false;
    }
  }

  copyCliq(): void {
    navigator.clipboard.writeText('BASILFODOULY').then(() => {
      this.cliqCopied = true;
      setTimeout(() => this.cliqCopied = false, 2000);
    });
  }

  onPaymentProofSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      
      // Validate file size (max 5MB)
      if (file.size > 5 * 1024 * 1024) {
        const errorMsg = this.currentLang === 'ar'
          ? 'حجم الصورة يجب أن يكون أقل من 5 ميجابايت'
          : 'Image size must be less than 5MB';
        alert(errorMsg);
        return;
      }
      
      // Validate file type
      if (!file.type.startsWith('image/')) {
        const errorMsg = this.currentLang === 'ar'
          ? 'يرجى اختيار ملف صورة'
          : 'Please select an image file';
        alert(errorMsg);
        return;
      }
      
      // Read file as base64
      const reader = new FileReader();
      reader.onload = (e: ProgressEvent<FileReader>) => {
        if (e.target?.result) {
          const base64String = e.target.result as string;
          this.orderForm.paymentProofImageUrl = base64String;
          this.paymentProofPreview = base64String;
        }
      };
      reader.readAsDataURL(file);
    }
  }

  removePaymentProof(): void {
    this.orderForm.paymentProofImageUrl = '';
    this.paymentProofPreview = null;
    // Reset file input
    const fileInput = document.getElementById('paymentProofInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }

  goToLogin(): void {
    this.router.navigate(['/login'], { queryParams: { returnUrl: '/checkout' } });
  }

  placeOrder(): void {
    if (!this.validateForm()) {
      return;
    }

    this.isPlacingOrder = true;

    // Debug: Log the order form to verify payment proof is included
    console.log('Placing order with data:', {
      hasPaymentProof: !!this.orderForm.paymentProofImageUrl,
      paymentProofLength: this.orderForm.paymentProofImageUrl?.length || 0
    });

    this.orderService.createOrderFromCart(this.orderForm)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (order) => {
          this.isPlacingOrder = false;
          
          console.log('Order created:', order);
          
          // Clear guest cart if not authenticated
          if (!this.isAuthenticated) {
            this.guestCartService.clearCart();
          }
          
          // Navigate to order confirmation
          this.router.navigate(['/order-confirmation', order.orderNumber]);
        },
        error: (error) => {
          console.error('Error placing order:', error);
          this.isPlacingOrder = false;
          const errorMsg = this.currentLang === 'ar'
            ? 'حدث خطأ أثناء تقديم الطلب. يرجى المحاولة مرة أخرى.'
            : 'Error placing order. Please try again.';
          alert(errorMsg);
        }
      });
  }

  private validateForm(): boolean {
    // For guest checkout, validate email and name
    if (!this.isAuthenticated) {
      if (!this.orderForm.guestEmail || !this.orderForm.guestName) {
        const errorMsg = this.currentLang === 'ar'
          ? 'الرجاء إدخال البريد الإلكتروني والاسم'
          : 'Please enter email and name';
        alert(errorMsg);
        return false;
      }
    }

    // Validate required fields
    if (!this.orderForm.shippingFirstName || !this.orderForm.shippingLastName ||
        !this.orderForm.shippingAddress || !this.orderForm.shippingCity ||
        !this.orderForm.shippingPhone) {
      const errorMsg = this.currentLang === 'ar'
        ? 'الرجاء ملء جميع الحقول المطلوبة'
        : 'Please fill in all required fields';
      alert(errorMsg);
      return false;
    }

    return true;
  }

  getCartTotal(): number {
    if (!this.cart) return 0;
    return this.cart.totalAmount + 3; // Total + 3 JOD shipping
  }

  getCartSubtotal(): number {
    if (!this.cart) return 0;
    return this.cart.totalAmount;
  }
}

