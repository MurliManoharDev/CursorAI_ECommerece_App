import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { OrderService, OrderDto } from '../../services/order.service';

@Component({
  selector: 'app-order-success',
  templateUrl: './order-success.component.html',
  styleUrls: ['./order-success.component.scss']
})
export class OrderSuccessComponent implements OnInit {
  order: OrderDto | null = null;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private orderService: OrderService
  ) {}

  ngOnInit(): void {
    const orderNumber = this.route.snapshot.paramMap.get('orderNumber');
    if (orderNumber) {
      this.loadOrder(orderNumber);
    } else {
      this.router.navigate(['/']);
    }
  }

  loadOrder(orderNumber: string): void {
    this.orderService.getOrderByNumber(orderNumber).subscribe({
      next: (order) => {
        this.order = order;
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load order:', error);
        this.loading = false;
        this.router.navigate(['/']);
      }
    });
  }

  getPaymentMethodDisplay(method: string): string {
    const methods: { [key: string]: string } = {
      'card': 'Credit/Debit Card',
      'bank-transfer': 'Bank Transfer',
      'cod': 'Cash on Delivery',
      'paypal': 'PayPal'
    };
    return methods[method] || method;
  }
}
