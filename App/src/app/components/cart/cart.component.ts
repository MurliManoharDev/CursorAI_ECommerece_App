import { Component, OnInit, inject } from '@angular/core';
import { Observable } from 'rxjs';
import { CartService, CartItem, CartSummary } from '../../services/cart.service';

@Component({
  selector: 'app-cart',
  templateUrl: './cart.component.html',
  styleUrls: ['./cart.component.scss']
})
export class CartComponent implements OnInit {
  cartItems$!: Observable<CartItem[]>;
  cartSummary$!: Observable<CartSummary>;

  private cartService = inject(CartService);

  ngOnInit(): void {
    this.cartItems$ = this.cartService.cartItems$;
    this.cartSummary$ = this.cartService.cartSummary$;
  }

  increaseQuantity(item: CartItem): void {
    this.cartService.updateQuantity(item.id, item.quantity + 1);
  }

  decreaseQuantity(item: CartItem): void {
    if (item.quantity > 1) {
      this.cartService.updateQuantity(item.id, item.quantity - 1);
    }
  }

  removeItem(item: CartItem): void {
    if (confirm(`Are you sure you want to remove ${item.name} from your cart?`)) {
      this.cartService.removeFromCart(item.id);
    }
  }
}
