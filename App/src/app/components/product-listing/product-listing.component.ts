import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormControl } from '@angular/forms';
import { Subject, debounceTime, distinctUntilChanged, takeUntil } from 'rxjs';
import { ProductService, Product, Category, ProductFilters, PagedResult } from '../../services/product.service';
import { CartService } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';

// Icon mapping for categories
const CATEGORY_ICON_MAP: { [key: string]: string } = {
  'laptops': 'fa-laptop',
  'pc-computers': 'fa-desktop',
  'pc & computers': 'fa-desktop',
  'computers': 'fa-desktop',
  'cell-phones': 'fa-mobile-alt',
  'cell phones': 'fa-mobile-alt',
  'mobile phones': 'fa-mobile-alt',
  'phones': 'fa-mobile-alt',
  'tablets': 'fa-tablet-alt',
  'gaming-vr': 'fa-gamepad',
  'gaming & vr': 'fa-gamepad',
  'gaming': 'fa-gamepad',
  'networking': 'fa-wifi',
  'cameras': 'fa-camera',
  'sounds': 'fa-headphones',
  'audio': 'fa-headphones',
  'headphones': 'fa-headphones',
  'office': 'fa-briefcase',
  'storage-usb': 'fa-hdd',
  'storage': 'fa-hdd',
  'usb': 'fa-usb',
  'accessories': 'fa-plug',
  'clearance': 'fa-gem',
  'sale': 'fa-tags',
  'default': 'fa-th'
};

@Component({
  selector: 'app-product-listing',
  templateUrl: './product-listing.component.html',
  styleUrls: ['./product-listing.component.scss']
})
export class ProductListingComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  
  // Services
  private productService = inject(ProductService);
  private cartService = inject(CartService);
  private wishlistService = inject(WishlistService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);

  // Data
  products: Product[] = [];
  categories: Category[] = [];
  pagedResult: PagedResult<Product> | null = null;
  
  // UI State
  isLoading = false;
  isCategoriesLoading = false;
  errorMessage = '';
  viewMode: 'grid' | 'list' = 'grid';
  selectedCategoryId: number | null = null;
  selectedSubcategoryId: number | null = null;
  expandedCategories: Set<number> = new Set();
  wishlistItems: Set<number> = new Set(); // Track wishlist items by ID
  
  // Search and Filters
  searchControl = new FormControl('');
  sortControl = new FormControl('featured');
  filters: ProductFilters = {
    pageNumber: 1,
    pageSize: 12,
    sortBy: 'featured'
  };

  // Sort options
  sortOptions = [
    { value: 'featured', label: 'Featured' },
    { value: 'name', label: 'Name (A-Z)' },
    { value: 'name-desc', label: 'Name (Z-A)' },
    { value: 'price', label: 'Price (Low to High)' },
    { value: 'price-desc', label: 'Price (High to Low)' },
    { value: 'rating', label: 'Highest Rated' },
    { value: 'newest', label: 'Newest First' }
  ];

  ngOnInit(): void {
    // Initialize wishlist items
    const currentWishlist = this.wishlistService.getWishlistItems();
    this.wishlistItems = new Set(currentWishlist.map(item => item.id));
    
    this.loadCategories();
    this.setupSearchListener();
    this.setupSortListener();
    
    // Subscribe to route params for category/subcategory navigation
    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      // Wait for categories to load before handling route
      if (this.categories.length === 0) {
        this.categoriesLoadedSubject.pipe(
          takeUntil(this.destroy$)
        ).subscribe(() => {
          this.handleRouteParams(params);
        });
      } else {
        this.handleRouteParams(params);
      }
    });

    // Subscribe to query params for search
    this.route.queryParams.pipe(takeUntil(this.destroy$)).subscribe(params => {
      let shouldReload = false;
      
      if (params['search']) {
        this.searchControl.setValue(params['search'], { emitEvent: false });
        this.filters.searchTerm = params['search'];
        shouldReload = true;
      }
      
      if (params['brand']) {
        this.filters.brandId = +params['brand'];
        shouldReload = true;
      } else {
        delete this.filters.brandId;
      }
      
      if (shouldReload) {
        this.loadProducts();
      }
    });
  }

  // Subject to notify when categories are loaded
  private categoriesLoadedSubject = new Subject<void>();

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    this.categoriesLoadedSubject.complete();
  }

  private handleRouteParams(params: any): void {
    if (params['category']) {
      this.handleCategoryRoute(params['category']);
    } else if (params['subcategory']) {
      this.handleSubcategoryRoute(params['subcategory']);
    } else {
      this.loadProducts();
    }
  }

  private setupSearchListener(): void {
    this.searchControl.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged(),
        takeUntil(this.destroy$)
      )
      .subscribe(searchTerm => {
        this.filters.searchTerm = searchTerm || undefined;
        this.filters.pageNumber = 1; // Reset to first page on new search
        this.updateQueryParams();
        this.loadProducts();
      });
  }

  private setupSortListener(): void {
    this.sortControl.valueChanges
      .pipe(takeUntil(this.destroy$))
      .subscribe(sortValue => {
        if (sortValue) {
          this.applySort(sortValue);
        }
      });
  }

  private loadCategories(): void {
    this.isCategoriesLoading = true;
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.isCategoriesLoading = false;
        this.categoriesLoadedSubject.next();
        
        // Auto-expand categories that have the selected subcategory
        if (this.selectedSubcategoryId) {
          const category = this.categories.find(cat => 
            cat.subcategories.some(sub => sub.id === this.selectedSubcategoryId)
          );
          if (category) {
            this.expandedCategories.add(category.id);
          }
        }
      },
      error: (error) => {
        console.error('Failed to load categories:', error);
        this.isCategoriesLoading = false;
      }
    });
  }

  // Get icon for category based on name
  getCategoryIcon(category: Category): string {
    if (category.iconClass) {
      return category.iconClass;
    }
    
    const nameLower = category.name.toLowerCase();
    const slugLower = category.slug.toLowerCase();
    
    // Try to find a match in the icon map
    return CATEGORY_ICON_MAP[slugLower] || 
           CATEGORY_ICON_MAP[nameLower] || 
           CATEGORY_ICON_MAP['default'];
  }

  private handleCategoryRoute(categorySlug: string): void {
    const category = this.categories.find(c => c.slug === categorySlug);
    if (category) {
      this.selectCategory(category.id);
    }
  }

  private handleSubcategoryRoute(subcategorySlug: string): void {
    // Find subcategory in all categories
    for (const category of this.categories) {
      const subcategory = category.subcategories.find(sub => sub.slug === subcategorySlug);
      if (subcategory) {
        this.selectSubcategory(category.id, subcategory.id);
        break;
      }
    }
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.productService.getProducts(this.filters).subscribe({
      next: (result) => {
        this.pagedResult = result;
        this.products = result.items;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load products:', error);
        this.errorMessage = 'Failed to load products. Please try again.';
        this.isLoading = false;
      }
    });
  }

  selectCategory(categoryId: number): void {
    if (this.selectedCategoryId === categoryId) {
      // Deselect if clicking the same category
      this.selectedCategoryId = null;
      this.filters.categoryId = undefined;
    } else {
      this.selectedCategoryId = categoryId;
      this.filters.categoryId = categoryId;
      this.expandedCategories.add(categoryId);
    }
    
    // Reset subcategory selection
    this.selectedSubcategoryId = null;
    this.filters.subcategoryId = undefined;
    this.filters.pageNumber = 1;
    
    this.loadProducts();
  }

  selectSubcategory(categoryId: number, subcategoryId: number): void {
    this.selectedCategoryId = categoryId;
    this.selectedSubcategoryId = subcategoryId;
    this.filters.categoryId = categoryId;
    this.filters.subcategoryId = subcategoryId;
    this.filters.pageNumber = 1;
    this.expandedCategories.add(categoryId);
    
    this.loadProducts();
  }

  toggleCategoryExpansion(categoryId: number): void {
    if (this.expandedCategories.has(categoryId)) {
      this.expandedCategories.delete(categoryId);
    } else {
      this.expandedCategories.add(categoryId);
    }
  }

  isCategoryExpanded(categoryId: number): boolean {
    return this.expandedCategories.has(categoryId);
  }

  clearFilters(): void {
    this.selectedCategoryId = null;
    this.selectedSubcategoryId = null;
    this.searchControl.setValue('');
    this.sortControl.setValue('featured');
    this.filters = {
      pageNumber: 1,
      pageSize: 12,
      sortBy: 'featured'
    };
    this.updateQueryParams();
    this.loadProducts();
  }

  applySort(sortValue: string): void {
    switch (sortValue) {
      case 'name':
        this.filters.sortBy = 'name';
        this.filters.sortDescending = false;
        break;
      case 'name-desc':
        this.filters.sortBy = 'name';
        this.filters.sortDescending = true;
        break;
      case 'price':
        this.filters.sortBy = 'price';
        this.filters.sortDescending = false;
        break;
      case 'price-desc':
        this.filters.sortBy = 'price';
        this.filters.sortDescending = true;
        break;
      case 'rating':
        this.filters.sortBy = 'rating';
        this.filters.sortDescending = true;
        break;
      case 'newest':
        this.filters.sortBy = 'createdAt';
        this.filters.sortDescending = true;
        break;
      default:
        this.filters.sortBy = 'featured';
        this.filters.sortDescending = false;
    }
    this.filters.pageNumber = 1;
    this.loadProducts();
  }

  setViewMode(mode: 'grid' | 'list'): void {
    this.viewMode = mode;
  }

  onPageChange(page: number): void {
    this.filters.pageNumber = page;
    this.loadProducts();
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  navigateToProduct(product: Product): void {
    this.router.navigate(['/product', product.id]);
  }

  viewProduct(product: any): void {
    this.router.navigate(['/product', product.id]);
  }

  private updateQueryParams(): void {
    const queryParams: any = {};
    if (this.filters.searchTerm) {
      queryParams.search = this.filters.searchTerm;
    }
    if (this.filters.brandId) {
      queryParams.brand = this.filters.brandId;
    }
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams,
      queryParamsHandling: 'merge'
    });
  }

  // Helper methods for template
  getDiscountPercentage(price: number, oldPrice: number): number {
    return Math.round(((oldPrice - price) / oldPrice) * 100);
  }

  getRatingStars(rating: number): string[] {
    const stars = [];
    const fullStars = Math.floor(rating);
    const hasHalfStar = rating % 1 >= 0.5;
    
    for (let i = 0; i < fullStars; i++) {
      stars.push('fas fa-star');
    }
    if (hasHalfStar) {
      stars.push('fas fa-star-half-alt');
    }
    const emptyStars = 5 - stars.length;
    for (let i = 0; i < emptyStars; i++) {
      stars.push('far fa-star');
    }
    
    return stars;
  }

  // Math helper for template
  getMinValue(a: number, b: number): number {
    return Math.min(a, b);
  }

  // Generate page numbers for pagination
  getPageNumbers(): number[] {
    if (!this.pagedResult) return [];
    
    const pages: number[] = [];
    const currentPage = this.pagedResult.pageNumber;
    const totalPages = this.pagedResult.totalPages;
    
    // Show up to 5 page numbers
    const startPage = Math.max(1, currentPage - 2);
    const endPage = Math.min(totalPages, currentPage + 2);
    
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
    
    return pages;
  }

  // Add product to cart
  addToCart(event: Event, product: Product): void {
    event.stopPropagation(); // Prevent navigating to product detail
    
    // Convert Product to CartItem
    const cartItem: any = {
      id: product.id,
      name: product.name,
      price: product.price,
      originalPrice: product.oldPrice,
      image: product.imageUrl,
      quantity: 1,
      inStock: true,
      shipping: product.freeShipping ? 'free shipping' : undefined
    };
    
    // Add to cart
    this.cartService.addToCart(cartItem);
    
    // You can add a success notification here if desired
    console.log(`${product.name} added to cart`);
  }

  // Toggle product in wishlist
  toggleWishlist(event: Event, product: Product): void {
    event.stopPropagation(); // Prevent navigating to product detail
    
    const wishlistItem = {
      id: product.id,
      name: product.name,
      price: product.price,
      oldPrice: product.oldPrice,
      imageUrl: product.imageUrl,
      categoryName: product.categoryName
    };
    
    const isAdded = this.wishlistService.toggleWishlist(wishlistItem);
    
    if (isAdded) {
      this.wishlistItems.add(product.id);
      console.log(`${product.name} added to wishlist`);
    } else {
      this.wishlistItems.delete(product.id);
      console.log(`${product.name} removed from wishlist`);
    }
  }

  // Check if product is in wishlist
  isInWishlist(productId: number): boolean {
    return this.wishlistItems.has(productId);
  }
}
