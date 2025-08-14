import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

export interface CreateOrderDto {
  firstName: string;
  lastName: string;
  companyName?: string;
  email: string;
  phoneNumber: string;
  streetAddress: string;
  apartmentSuite?: string;
  city: string;
  state: string;
  country: string;
  zipCode: string;
  orderNotes?: string;
  paymentMethod: string;
  paymentIntentId?: string;
  createAccount: boolean;
  password?: string;
  cartItems: CartItemCreateDto[];
}

export interface CartItemCreateDto {
  productId: number;
  productName: string;
  productImage?: string;
  quantity: number;
  price: number;
  shipping?: string;
  shippingCost?: number;
}

export interface OrderDto {
  id: number;
  orderNumber: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  streetAddress: string;
  apartmentSuite?: string;
  city: string;
  state: string;
  country: string;
  zipCode: string;
  subtotal: number;
  shippingCost: number;
  tax: number;
  total: number;
  orderNotes?: string;
  paymentMethod: string;
  paymentStatus: string;
  status: string;
  createdAt: Date;
  updatedAt: Date;
  shippedAt?: Date;
  deliveredAt?: Date;
  orderItems: OrderItemDto[];
  statusHistory: OrderStatusHistoryDto[];
}

export interface OrderItemDto {
  id: number;
  productId: number;
  productName: string;
  productImage?: string;
  quantity: number;
  price: number;
  total: number;
  shippingType?: string;
  shippingCost: number;
}

export interface OrderStatusHistoryDto {
  id: number;
  status: string;
  notes?: string;
  userName?: string;
  createdAt: Date;
}

export interface OrderListDto {
  id: number;
  orderNumber: string;
  total: number;
  status: string;
  paymentStatus: string;
  createdAt: Date;
  itemCount: number;
}

export interface PaymentIntentDto {
  clientSecret: string;
  paymentIntentId: string;
  amount: number;
}

@Injectable({
  providedIn: 'root'
})
export class OrderService {
  private apiUrl = `${environment.apiUrl}/orders`;

  constructor(
    private http: HttpClient,
    private authService: AuthService
  ) { }

  createOrder(orderData: CreateOrderDto): Observable<OrderDto> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post<OrderDto>(`${this.apiUrl}/create`, orderData, { headers });
  }

  createPaymentIntent(amount: number): Observable<PaymentIntentDto> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post<PaymentIntentDto>(`${this.apiUrl}/payment-intent`, { amount }, { headers });
  }

  confirmPayment(orderId: number, paymentIntentId: string): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post(`${this.apiUrl}/${orderId}/confirm-payment`, { paymentIntentId }, { headers });
  }

  getOrder(orderId: number): Observable<OrderDto> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<OrderDto>(`${this.apiUrl}/${orderId}`, { headers });
  }

  getOrderByNumber(orderNumber: string): Observable<OrderDto> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<OrderDto>(`${this.apiUrl}/order-number/${orderNumber}`, { headers });
  }

  getMyOrders(): Observable<OrderListDto[]> {
    const headers = this.authService.getAuthHeaders();
    return this.http.get<OrderListDto[]>(`${this.apiUrl}/my-orders`, { headers });
  }

  updateOrderStatus(orderId: number, status: string, notes?: string): Observable<OrderDto> {
    const headers = this.authService.getAuthHeaders();
    return this.http.put<OrderDto>(`${this.apiUrl}/${orderId}/status`, { status, notes }, { headers });
  }

  cancelOrder(orderId: number): Observable<any> {
    const headers = this.authService.getAuthHeaders();
    return this.http.post(`${this.apiUrl}/${orderId}/cancel`, {}, { headers });
  }
} 