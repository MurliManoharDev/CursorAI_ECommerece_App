import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface CartItem {
  id: number;
  name: string;
  price: number;
  originalPrice?: number;
  image: string;
  quantity: number;
  rating?: number;
  reviews?: number;
  shipping?: string;
  shippingCost?: number;
  inStock: boolean;
  badges?: string[];
  discount?: number;
}

export interface CartSummary {
  subtotal: number;
  shippingEstimate: number;
  taxEstimate: number;
  total: number;
  itemCount: number;
}

@Injectable({
  providedIn: 'root'
})
export class CartService {
  private cartItemsSubject: BehaviorSubject<CartItem[]>;
  private cartSummarySubject: BehaviorSubject<CartSummary>;
  
  public cartItems$: Observable<CartItem[]>;
  public cartSummary$: Observable<CartSummary>;

  // Mock initial cart items - in real app, this would come from backend or localStorage
  private mockCartItems: CartItem[] = [];

  constructor() {
    // Initialize with mock data
    this.cartItemsSubject = new BehaviorSubject<CartItem[]>(this.mockCartItems);
    this.cartSummarySubject = new BehaviorSubject<CartSummary>(this.calculateSummary(this.mockCartItems));
    
    this.cartItems$ = this.cartItemsSubject.asObservable();
    this.cartSummary$ = this.cartSummarySubject.asObservable();
  }

  get cartItems(): CartItem[] {
    return this.cartItemsSubject.value;
  }

  get cartSummary(): CartSummary {
    return this.cartSummarySubject.value;
  }

  addToCart(item: CartItem): void {
    const currentItems = [...this.cartItems];
    const existingItem = currentItems.find(i => i.id === item.id);
    
    if (existingItem) {
      existingItem.quantity += item.quantity;
    } else {
      currentItems.push(item);
    }
    
    this.updateCart(currentItems);
  }

  updateQuantity(itemId: number, quantity: number): void {
    if (quantity < 1) return;
    
    const currentItems = [...this.cartItems];
    const item = currentItems.find(i => i.id === itemId);
    
    if (item) {
      item.quantity = quantity;
      this.updateCart(currentItems);
    }
  }

  removeFromCart(itemId: number): void {
    const currentItems = this.cartItems.filter(item => item.id !== itemId);
    this.updateCart(currentItems);
  }

  clearCart(): void {
    this.updateCart([]);
  }

  private updateCart(items: CartItem[]): void {
    this.cartItemsSubject.next(items);
    this.cartSummarySubject.next(this.calculateSummary(items));
  }

  private calculateSummary(items: CartItem[]): CartSummary {
    const subtotal = items.reduce((sum, item) => sum + (item.price * item.quantity), 0);
    const shippingEstimate = items.reduce((sum, item) => {
      return sum + (item.shippingCost || 0) * item.quantity;
    }, 0);
    const taxRate = 0.08; // 8% tax rate
    const taxEstimate = (subtotal + shippingEstimate) * taxRate;
    const total = subtotal + shippingEstimate + taxEstimate;
    const itemCount = items.reduce((count, item) => count + item.quantity, 0);

    return {
      subtotal,
      shippingEstimate,
      taxEstimate,
      total,
      itemCount
    };
  }
}
