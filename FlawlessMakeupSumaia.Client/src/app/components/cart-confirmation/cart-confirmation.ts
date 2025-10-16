import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';

export interface CartConfirmationData {
  productName: string;
  productImage: string;
  cartItemCount: number;
}

@Component({
  selector: 'app-cart-confirmation',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  templateUrl: './cart-confirmation.html',
  styleUrl: './cart-confirmation.scss'
})
export class CartConfirmationComponent {
  @Input() show: boolean = false;
  @Input() data: CartConfirmationData | null = null;
  @Output() closed = new EventEmitter<void>();

  close(): void {
    this.closed.emit();
  }

  viewCart(): void {
    this.close();
  }

  checkout(): void {
    this.close();
  }

  continueShopping(): void {
    this.close();
  }
}


