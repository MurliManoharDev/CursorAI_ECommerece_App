import { Component, OnInit, OnDestroy, inject } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService, Category } from '../../services/product.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-main-menu',
  templateUrl: './main-menu.component.html',
  styleUrls: ['./main-menu.component.scss']
})
export class MainMenuComponent implements OnInit, OnDestroy {
  // Search properties
  searchQuery = '';
  selectedSearchCategory = '';
  searchCategories: Category[] = [];
  showSuggestions = false;
  searchSuggestions: any[] = [];
  private searchTimeout: any;
  
  private categorySubscription?: Subscription;
  
  private router = inject(Router);
  private productService = inject(ProductService);
  
  ngOnInit(): void {
    this.loadCategories();
  }
  
  ngOnDestroy(): void {
    if (this.categorySubscription) {
      this.categorySubscription.unsubscribe();
    }
    if (this.searchTimeout) {
      clearTimeout(this.searchTimeout);
    }
  }
  
  loadCategories(): void {
    this.categorySubscription = this.productService.getCategories().subscribe({
      next: (categories) => {
        this.searchCategories = categories;
      },
      error: (error) => {
        console.error('Failed to load categories:', error);
      }
    });
  }
  
  // Search methods
  performSearch(): void {
    if (this.searchQuery.trim()) {
      const params: any = { q: this.searchQuery };
      if (this.selectedSearchCategory) {
        params.category = this.selectedSearchCategory;
      }
      this.router.navigate(['/products'], { queryParams: params });
      this.clearSearch();
    }
  }
  
  onSearchInputChange(): void {
    // Clear previous timeout
    if (this.searchTimeout) {
      clearTimeout(this.searchTimeout);
    }
    
    // Hide suggestions if query is empty
    if (!this.searchQuery.trim()) {
      this.showSuggestions = false;
      this.searchSuggestions = [];
      return;
    }
    
    // Set a new timeout for debounced search
    this.searchTimeout = setTimeout(() => {
      this.fetchSearchSuggestions();
    }, 300);
  }
  
  fetchSearchSuggestions(): void {
    if (this.searchQuery.trim().length < 2) {
      return;
    }
    
    this.productService.searchProducts(this.searchQuery, this.selectedSearchCategory).subscribe({
      next: (results) => {
        this.searchSuggestions = results.slice(0, 5).map(product => ({
          id: product.id,
          name: product.name,
          category: product.categoryName || '',
          slug: product.slug
        }));
        this.showSuggestions = this.searchSuggestions.length > 0;
      },
      error: (error) => {
        console.error('Search failed:', error);
        this.searchSuggestions = [];
        this.showSuggestions = false;
      }
    });
  }
  
  selectSuggestion(suggestion: any): void {
    this.router.navigate(['/product', suggestion.id]);
    this.clearSearch();
  }
  
  clearSearch(): void {
    this.searchQuery = '';
    this.showSuggestions = false;
    this.searchSuggestions = [];
  }
  
  onSearchFocus(): void {
    if (this.searchQuery.trim() && this.searchSuggestions.length > 0) {
      this.showSuggestions = true;
    }
  }
  
  onSearchBlur(): void {
    // Delay hiding to allow click on suggestions
    setTimeout(() => {
      this.showSuggestions = false;
    }, 200);
  }
}
