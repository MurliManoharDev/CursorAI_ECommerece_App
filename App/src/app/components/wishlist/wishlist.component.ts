import { Component, OnInit, inject } from '@angular/core';
import { Router } from '@angular/router';
import { WishlistService, WishlistItem } from '../../services/wishlist.service';
import { CartService } from '../../services/cart.service';

@Component({
  selector: 'app-wishlist',
  templateUrl: './wishlist.component.html',
  styleUrls: ['./wishlist.component.scss']
})
export class WishlistComponent implements OnInit {
  private wishlistService = inject(WishlistService);
  private cartService = inject(CartService);
  private router = inject(Router);
  
  wishlistItems: WishlistItem[] = [];

  ngOnInit(): void {
    this.loadWishlistItems();
  }

  loadWishlistItems(): void {
    this.wishlistItems = this.wishlistService.getWishlistItems();
  }

  removeFromWishlist(productId: number): void {
    this.wishlistService.removeFromWishlist(productId);
    this.loadWishlistItems();
  }

  addToCart(item: WishlistItem): void {
    const cartItem = {
      id: item.id,
      name: item.name,
      price: item.price,
      originalPrice: item.oldPrice,
      image: item.imageUrl,
      quantity: 1,
      inStock: true
    };
    
    this.cartService.addToCart(cartItem);
    console.log(`${item.name} added to cart`);
  }

  moveToCart(item: WishlistItem): void {
    this.addToCart(item);
    this.removeFromWishlist(item.id);
  }

  clearWishlist(): void {
    if (confirm('Are you sure you want to clear your wishlist?')) {
      this.wishlistService.clearWishlist();
      this.loadWishlistItems();
    }
  }

  navigateToProduct(productId: number): void {
    this.router.navigate(['/product', productId]);
  }

  continueShopping(): void {
    this.router.navigate(['/products']);
  }
} 