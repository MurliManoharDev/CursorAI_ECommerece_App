import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { OrderService, OrderListDto, OrderDto } from '../../services/order.service';

@Component({
  selector: 'app-my-orders',
  templateUrl: './my-orders.component.html',
  styleUrls: ['./my-orders.component.scss']
})
export class MyOrdersComponent implements OnInit {
  orders: OrderListDto[] = [];
  loading = true;
  selectedOrderId: number | null = null;
  orderDetails: OrderDto | null = null;

  constructor(
    private orderService: OrderService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadOrders();
  }

  loadOrders(): void {
    this.orderService.getMyOrders().subscribe({
      next: (orders) => {
        this.orders = orders;
        this.loading = false;
      },
      error: (error) => {
        console.error('Failed to load orders:', error);
        this.loading = false;
      }
    });
  }

  viewOrder(orderId: number): void {
    if (this.selectedOrderId === orderId) {
      this.selectedOrderId = null;
      this.orderDetails = null;
    } else {
      this.selectedOrderId = orderId;
      this.loadOrderDetails(orderId);
    }
  }

  loadOrderDetails(orderId: number): void {
    this.orderService.getOrder(orderId).subscribe({
      next: (order) => {
        this.orderDetails = order;
      },
      error: (error) => {
        console.error('Failed to load order details:', error);
      }
    });
  }

  trackOrder(order: OrderListDto): void {
    // In a real app, this would open a tracking page or modal
    alert(`Tracking information for order ${order.orderNumber} will be available soon.`);
  }

  cancelOrder(order: OrderListDto): void {
    if (confirm(`Are you sure you want to cancel order ${order.orderNumber}?`)) {
      this.orderService.cancelOrder(order.id).subscribe({
        next: () => {
          alert('Order cancelled successfully');
          this.loadOrders();
        },
        error: (error) => {
          alert('Failed to cancel order. Please try again.');
          console.error('Cancel order error:', error);
        }
      });
    }
  }

  canTrackOrder(status: string): boolean {
    return ['Confirmed', 'Shipped'].includes(status);
  }

  canCancelOrder(status: string): boolean {
    return status === 'Pending';
  }

  getStatusClass(status: string): string {
    const statusClasses: { [key: string]: string } = {
      'Pending': 'status-pending',
      'Processing': 'status-processing',
      'Confirmed': 'status-confirmed',
      'Shipped': 'status-shipped',
      'Delivered': 'status-delivered',
      'Cancelled': 'status-cancelled',
      'Refunded': 'status-refunded'
    };
    return statusClasses[status] || 'status-default';
  }

  getStatusIcon(status: string): string {
    const statusIcons: { [key: string]: string } = {
      'Pending': 'fas fa-clock',
      'Processing': 'fas fa-spinner',
      'Confirmed': 'fas fa-check-circle',
      'Shipped': 'fas fa-shipping-fast',
      'Delivered': 'fas fa-box-check',
      'Cancelled': 'fas fa-times-circle',
      'Refunded': 'fas fa-undo'
    };
    return statusIcons[status] || 'fas fa-question-circle';
  }

  getStatusDisplay(status: string): string {
    return status;
  }

  getPaymentStatusClass(status: string): string {
    const statusClasses: { [key: string]: string } = {
      'Pending': 'payment-pending',
      'Processing': 'payment-processing',
      'Succeeded': 'payment-succeeded',
      'Failed': 'payment-failed',
      'Cancelled': 'payment-cancelled',
      'Refunded': 'payment-refunded'
    };
    return statusClasses[status] || 'payment-default';
  }

  getPaymentStatusDisplay(status: string): string {
    return status;
  }
}
