import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { Subject, takeUntil, forkJoin } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { AdminService } from '../../services/admin.service';
import { NotificationService } from '../../services/notification.service';
import { AdminDashboard, ProductAnalytics } from '../../models/admin.model';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.scss'
})
export class AdminDashboardComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  dashboard: AdminDashboard | null = null;
  analytics: ProductAnalytics | null = null;
  isLoading = true;

  constructor(
    private adminService: AdminService,
    private notificationService: NotificationService
  ) { }

  ngOnInit(): void {
    this.loadDashboardData();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadDashboardData(): void {
    this.isLoading = true;

    forkJoin({
      dashboard: this.adminService.getDashboard(),
      analytics: this.adminService.getProductAnalytics()
    }).pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (data) => {
          this.dashboard = data.dashboard;
          this.analytics = data.analytics;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading dashboard:', error);
          this.isLoading = false;
        }
      });
  }

  refreshDashboard(): void {
    this.loadDashboardData();
  }

  viewAnalytics(): void {
    // Toggle analytics visibility or navigate to detailed analytics page
    console.log('View detailed analytics');
  }

  testApiConnection(): void {
    console.log('Testing API connection...');
    this.adminService.getDashboard().subscribe({
      next: (data) => {
        console.log('API Response:', data);
        this.notificationService.success(`API Connection Successful!\nTotal Products: ${data.totalProducts}\nTotal Categories: ${data.totalCategories}`);
      },
      error: (error) => {
        console.error('API Connection Failed:', error);
        this.notificationService.error(`API Connection Failed: ${error.message || 'Unknown error'}`);
      }
    });
  }

  getStockPercentage(type: 'in-stock' | 'low-stock' | 'out-of-stock'): number {
    if (!this.dashboard) return 0;

    const total = this.dashboard.totalProducts;
    if (total === 0) return 0;

    const inStock = total - this.dashboard.lowStockProducts - this.dashboard.outOfStockProducts;

    switch (type) {
      case 'in-stock':
        return (inStock / total) * 100;
      case 'low-stock':
        return (this.dashboard.lowStockProducts / total) * 100;
      case 'out-of-stock':
        return (this.dashboard.outOfStockProducts / total) * 100;
      default:
        return 0;
    }
  }

  getCategoryPercentage(count: number): number {
    if (!this.analytics) return 0;
    return Math.round((count / this.analytics.totalProducts) * 100);
  }
}