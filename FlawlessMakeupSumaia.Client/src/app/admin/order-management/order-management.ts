import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { OrderService } from '../../services/order.service';
import { NotificationService } from '../../services/notification.service';
import { Order, OrderStatus } from '../../models/order.model';

@Component({
  selector: 'app-order-management',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, TranslateModule],
  templateUrl: './order-management.html',
  styleUrl: './order-management.scss'
})
export class OrderManagementComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  orders: Order[] = [];
  filteredOrders: Order[] = [];
  isLoading = true;
  currentLang = 'en';
  statusFilter: OrderStatus | 'all' = 'all';
  selectedOrder: Order | null = null;
  showDetailsModal = false;

  OrderStatus = OrderStatus;

  constructor(
    private orderService: OrderService,
    private notificationService: NotificationService,
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
    
    this.orderService.getAllOrdersForAdmin()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (orders) => {
          this.orders = orders;
          this.filterOrders();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading orders:', error);
          this.isLoading = false;
        }
      });
  }

  filterOrders(): void {
    if (this.statusFilter === 'all') {
      this.filteredOrders = this.orders;
    } else {
      this.filteredOrders = this.orders.filter(order => order.status === this.statusFilter);
    }
  }

  updateOrderStatus(order: Order, newStatus: any): void {
    // Convert string value from select to number
    const statusValue = typeof newStatus === 'string' ? parseInt(newStatus, 10) : newStatus;
    
    this.orderService.updateOrderStatus(order.id, statusValue as OrderStatus)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (updatedOrder) => {
          const index = this.orders.findIndex(o => o.id === order.id);
          if (index !== -1) {
            this.orders[index] = updatedOrder;
          }
          this.filterOrders();
        },
        error: (error) => {
          console.error('Error updating order status:', error);
          const errorMsg = this.currentLang === 'ar' 
            ? 'حدث خطأ أثناء تحديث حالة الطلب'
            : 'Error updating order status';
          this.notificationService.error(errorMsg);
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

  getStatusLabel(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending: return 'ADMIN_ORDERS.PENDING';
      case OrderStatus.Confirmed: return 'ADMIN_ORDERS.CONFIRMED';
      case OrderStatus.Completed: return 'ADMIN_ORDERS.COMPLETED';
      case OrderStatus.Cancelled: return 'ADMIN_ORDERS.CANCELLED';
      default: return '';
    }
  }

  getStatusClass(status: OrderStatus): string {
    switch (status) {
      case OrderStatus.Pending: return 'badge bg-warning';
      case OrderStatus.Confirmed: return 'badge bg-info';
      case OrderStatus.Completed: return 'badge bg-success';
      case OrderStatus.Cancelled: return 'badge bg-danger';
      default: return 'badge bg-secondary';
    }
  }

  getCustomerName(order: Order): string {
    if (order.userId) {
      return `${order.shippingFirstName} ${order.shippingLastName}`;
    } else {
      return order.guestName || `${order.shippingFirstName} ${order.shippingLastName}`;
    }
  }

  isGuestOrder(order: Order): boolean {
    return !order.userId;
  }
}

