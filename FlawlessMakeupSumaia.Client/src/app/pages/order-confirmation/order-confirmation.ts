import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { TranslateModule, TranslateService } from '@ngx-translate/core';

import { OrderService } from '../../services/order.service';
import { Order } from '../../models/order.model';

@Component({
  selector: 'app-order-confirmation',
  standalone: true,
  imports: [CommonModule, RouterModule, TranslateModule],
  templateUrl: './order-confirmation.html',
  styleUrl: './order-confirmation.scss'
})
export class OrderConfirmationComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  order: Order | null = null;
  isLoading = true;
  currentLang = 'en';
  cliqCopied = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService,
    private translate: TranslateService
  ) {
    this.currentLang = this.translate.currentLang || this.translate.defaultLang || 'en';
    
    this.translate.onLangChange.pipe(takeUntil(this.destroy$)).subscribe((event) => {
      this.currentLang = event.lang;
    });
  }

  ngOnInit(): void {
    this.route.params.pipe(takeUntil(this.destroy$))
      .subscribe(params => {
        const orderNumber = params['orderNumber'];
        if (orderNumber) {
          this.loadOrder(orderNumber);
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadOrder(orderNumber: string): void {
    this.orderService.getOrderByNumber(orderNumber)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (order) => {
          this.order = order;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading order:', error);
          this.isLoading = false;
          this.router.navigate(['/']);
        }
      });
  }

  copyCliq(): void {
    navigator.clipboard.writeText('SUMAIA1991').then(() => {
      this.cliqCopied = true;
      setTimeout(() => this.cliqCopied = false, 2000);
    });
  }
}

