import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { map } from 'rxjs/operators';

export interface WishlistItem {
  id: number;
  name: string;
  price: number;
  oldPrice?: number;
  imageUrl: string;
  categoryName: string;
  addedAt: Date;
}

@Injectable({
  providedIn: 'root'
})
export class WishlistService {
  private wishlistKey = 'ecommerce_wishlist';
  private wishlistItemsSubject: BehaviorSubject<WishlistItem[]>;
  public wishlistItems$: Observable<WishlistItem[]>;
  public wishlistCount$: Observable<number>;

  constructor() {
    // Load wishlist from localStorage
    const savedWishlist = this.loadFromLocalStorage();
    this.wishlistItemsSubject = new BehaviorSubject<WishlistItem[]>(savedWishlist);
    this.wishlistItems$ = this.wishlistItemsSubject.asObservable();
    this.wishlistCount$ = this.wishlistItems$.pipe(
      map(items => items.length)
    );
  }

  // Load wishlist from localStorage
  private loadFromLocalStorage(): WishlistItem[] {
    try {
      const saved = localStorage.getItem(this.wishlistKey);
      return saved ? JSON.parse(saved) : [];
    } catch {
      return [];
    }
  }

  // Save wishlist to localStorage
  private saveToLocalStorage(items: WishlistItem[]): void {
    localStorage.setItem(this.wishlistKey, JSON.stringify(items));
  }

  // Add item to wishlist
  addToWishlist(item: Omit<WishlistItem, 'addedAt'>): void {
    const currentItems = this.wishlistItemsSubject.value;
    
    // Check if item already exists
    if (!currentItems.find(i => i.id === item.id)) {
      const newItem: WishlistItem = {
        ...item,
        addedAt: new Date()
      };
      const updatedItems = [...currentItems, newItem];
      this.wishlistItemsSubject.next(updatedItems);
      this.saveToLocalStorage(updatedItems);
    }
  }

  // Remove item from wishlist
  removeFromWishlist(productId: number): void {
    const currentItems = this.wishlistItemsSubject.value;
    const updatedItems = currentItems.filter(item => item.id !== productId);
    this.wishlistItemsSubject.next(updatedItems);
    this.saveToLocalStorage(updatedItems);
  }

  // Toggle item in wishlist
  toggleWishlist(item: Omit<WishlistItem, 'addedAt'>): boolean {
    if (this.isInWishlist(item.id)) {
      this.removeFromWishlist(item.id);
      return false;
    } else {
      this.addToWishlist(item);
      return true;
    }
  }

  // Check if item is in wishlist
  isInWishlist(productId: number): boolean {
    return this.wishlistItemsSubject.value.some(item => item.id === productId);
  }

  // Clear all wishlist items
  clearWishlist(): void {
    this.wishlistItemsSubject.next([]);
    this.saveToLocalStorage([]);
  }

  // Get current wishlist items
  getWishlistItems(): WishlistItem[] {
    return this.wishlistItemsSubject.value;
  }
} 