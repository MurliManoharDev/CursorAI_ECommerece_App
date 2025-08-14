import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CartService, CartSummary } from '../../services/cart.service';
import { ProductService, Category } from '../../services/product.service';
import { WishlistService } from '../../services/wishlist.service';
import { Subscription } from 'rxjs';

interface MenuItem {
  id: string;
  title: string;
  slug: string;
  isActive: boolean;
  children: Category[];
}

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit, OnDestroy {
  isLoggedIn = false;
  userName = '';
  cartSummary: CartSummary | null = null;
  wishlistCount = 0;
  activeDropdown: string | null = null;
  isAccountDropdownOpen = false;
  menuItems: MenuItem[] = [];
  isCategoriesLoading = false;
  
  private authSubscription?: Subscription;
  private cartSubscription?: Subscription;
  private wishlistSubscription?: Subscription;

  private router = inject(Router);
  private authService = inject(AuthService);
  private cartService = inject(CartService);
  private productService = inject(ProductService);
  private wishlistService = inject(WishlistService);

  ngOnInit(): void {
    // Subscribe to auth state
    this.authSubscription = this.authService.currentUser$.subscribe(user => {
      this.isLoggedIn = !!user;
      this.userName = user?.name || '';
    });

    // Subscribe to cart state
    this.cartSubscription = this.cartService.cartSummary$.subscribe(summary => {
      this.cartSummary = summary;
    });

    // Subscribe to wishlist state
    this.wishlistSubscription = this.wishlistService.wishlistCount$.subscribe(count => {
      this.wishlistCount = count;
    });

    // Load categories
    this.loadCategories();
  }

  ngOnDestroy(): void {
    if (this.authSubscription) {
      this.authSubscription.unsubscribe();
    }
    if (this.cartSubscription) {
      this.cartSubscription.unsubscribe();
    }
    if (this.wishlistSubscription) {
      this.wishlistSubscription.unsubscribe();
    }
  }

  loadCategories(): void {
    this.isCategoriesLoading = true;
    this.productService.getCategories().subscribe({
      next: (categories) => {
        console.log('Raw categories from API:', categories);
        
        // Show only top 4 categories to avoid crowding the header
        // Categories without subcategories will just show the category link without a dropdown
        this.menuItems = categories
          .slice(0, 4)  // Limit to first 4 categories
          .map(category => ({
            id: category.slug,
            title: category.name,
            slug: category.slug,
            isActive: false,
            children: [category] // Wrap in array for consistent structure with mega dropdown
          }));
        
        console.log('Final menu items:', this.menuItems);
        console.log('Total categories received:', categories.length);
        console.log('Menu items shown in header:', this.menuItems.length);
        this.isCategoriesLoading = false;
      },
      error: (error) => {
        console.error('Failed to load categories:', error);
        this.isCategoriesLoading = false;
      }
    });
  }

  toggleDropdown(menuId: string): void {
    if (this.activeDropdown === menuId) {
      this.activeDropdown = null;
    } else {
      this.activeDropdown = menuId;
    }
  }

  closeDropdown(): void {
    this.activeDropdown = null;
    this.isAccountDropdownOpen = false;
  }

  isDropdownOpen(menuId: string): boolean {
    return this.activeDropdown === menuId;
  }

  navigateToCategory(categorySlug: string): void {
    this.router.navigate(['/products/category', categorySlug]);
    this.closeDropdown();
  }

  navigateToSubcategory(subcategorySlug: string): void {
    this.router.navigate(['/products/subcategory', subcategorySlug]);
    this.closeDropdown();
  }

  navigateToLogin(): void {
    this.router.navigate(['/login']);
  }

  logout(): void {
    this.authService.logout();
  }

  navigateToAccount(): void {
    if (this.isLoggedIn) {
      this.router.navigate(['/profile']);
    } else {
      this.navigateToLogin();
    }
  }

  navigateToCart(): void {
    this.router.navigate(['/cart']);
  }

  navigateToWishlist(): void {
    this.router.navigate(['/wishlist']);
  }

  toggleAccountDropdown(): void {
    this.isAccountDropdownOpen = !this.isAccountDropdownOpen;
    // Close category dropdowns when opening account dropdown
    this.activeDropdown = null;
  }

  closeAccountDropdown(): void {
    this.isAccountDropdownOpen = false;
  }
}
