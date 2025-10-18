import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { OrderService } from '../../services/order.service';
import { Order, OrderStatus } from '../../models/order.model';

@Component({
  selector: 'app-orders',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslateModule],
  templateUrl: './orders.html',
  styleUrl: './orders.scss'
})
export class OrdersComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  orders: Order[] = [];
  isLoading = true;
  currentLang = 'en';
  selectedOrder: Order | null = null;
  showDetailsModal = false;

  OrderStatus = OrderStatus;

  constructor(
    private orderService: OrderService,
    private translate: TranslateService
  ) {
    this.currentLang = this.translate.currentLang || this.translate.defaultLang || 'en';
    
    this.translate.onLangChange.pipe(takeUntil(this.destroy$)).subscribe((event) => {
      this.currentLang = event.lang;
    });
  }

  ngOnInit(): void {
    this.loadOrders();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  loadOrders(): void {
    this.isLoading = true;
    
    this.orderService.getOrders()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (orders) => {
          this.orders = orders.sort((a, b) => 
            new Date(b.orderDate).getTime() - new Date(a.orderDate).getTime()
          );
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading orders:', error);
          this.isLoading = false;
        }
      });
  }

  viewOrderDetails(order: Order): void {
    this.selectedOrder = order;
    this.showDetailsModal = true;
  }

  closeDetailsModal(): void {
    this.showDetailsModal = false;
    this.selectedOrder = null;
  }

  getStatusBadgeClass(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending:
        return 'bg-warning';
      case OrderStatus.Confirmed:
        return 'bg-info';
      case OrderStatus.Completed:
        return 'bg-success';
      case OrderStatus.Cancelled:
        return 'bg-danger';
      default:
        return 'bg-secondary';
    }
  }

  getStatusText(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending:
        return this.translate.instant('ORDER_STATUS.PENDING');
      case OrderStatus.Confirmed:
        return this.translate.instant('ORDER_STATUS.CONFIRMED');
      case OrderStatus.Completed:
        return this.translate.instant('ORDER_STATUS.COMPLETED');
      case OrderStatus.Cancelled:
        return this.translate.instant('ORDER_STATUS.CANCELLED');
      default:
        return '';
    }
  }

  formatDate(date: string | Date): string {
    return new Date(date).toLocaleDateString(this.currentLang === 'ar' ? 'ar-JO' : 'en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  getProductImageUrl(imageUrl: string | null | undefined): string {
    if (!imageUrl) {
      return 'assets/images/placeholder.jpg';
    }
    if (imageUrl.startsWith('http')) {
      return imageUrl;
    }
    return `http://localhost:5001/${imageUrl}`;
  }
}

