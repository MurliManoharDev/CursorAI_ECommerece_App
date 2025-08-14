import { Component, Input, inject, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { CartService } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';

@Component({
  selector: 'app-product-card',
  templateUrl: './product-card.component.html',
  styleUrls: ['./product-card.component.scss']
})
export class ProductCardComponent implements OnInit {
  @Input() product: any;
  
  private router = inject(Router);
  private cartService = inject(CartService);
  private wishlistService = inject(WishlistService);
  
  isInWishlist = false;

  ngOnInit(): void {
    // Check if product is in wishlist
    if (this.product) {
      this.isInWishlist = this.wishlistService.isInWishlist(this.product.id);
    }
  }

  navigateToProduct(): void {
    if (this.product && this.product.id) {
      this.router.navigate(['/product', this.product.id]);
    }
  }

  addToCart(event: Event): void {
    event.stopPropagation();
    
    const cartItem = {
      id: this.product.id,
      name: this.product.name,
      price: this.product.price,
      originalPrice: this.product.oldPrice || this.product.originalPrice,
      image: this.product.imageUrl || this.product.image,
      quantity: 1,
      inStock: true,
      shipping: this.product.freeShipping ? 'free shipping' : undefined
    };
    
    this.cartService.addToCart(cartItem);
    console.log(`${this.product.name} added to cart`);
  }

  toggleWishlist(event: Event): void {
    event.stopPropagation();
    
    const wishlistItem = {
      id: this.product.id,
      name: this.product.name,
      price: this.product.price,
      oldPrice: this.product.oldPrice || this.product.originalPrice,
      imageUrl: this.product.imageUrl || this.product.image,
      categoryName: this.product.categoryName || 'Electronics'
    };
    
    this.isInWishlist = this.wishlistService.toggleWishlist(wishlistItem);
    console.log(`${this.product.name} ${this.isInWishlist ? 'added to' : 'removed from'} wishlist`);
  }
}
