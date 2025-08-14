import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

export interface Product {
  id: number;
  name: string;
  subtitle?: string;
  slug: string;
  imageUrl: string;
  price: number;
  oldPrice?: number;
  brandName?: string;
  categoryName: string;
  isOnSale: boolean;
  freeShipping: boolean;
  averageRating?: number;
  reviewCount: number;
}

export interface Category {
  id: number;
  name: string;
  slug: string;
  iconClass?: string;
  imageUrl?: string;
  productCount: number;
  subcategories: Subcategory[];
}

export interface Subcategory {
  id: number;
  name: string;
  slug: string;
  iconClass?: string;
  itemCount: number;
}

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

export interface PaginationParams {
  pageNumber?: number;
  pageSize?: number;
  sortBy?: string;
  sortDescending?: boolean;
  searchTerm?: string;
}

export interface ProductFilters extends PaginationParams {
  categoryId?: number;
  subcategoryId?: number;
  brandId?: number;
  minPrice?: number;
  maxPrice?: number;
  inStock?: boolean;
  freeShipping?: boolean;
  isOnSale?: boolean;
}

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private apiUrl = environment.apiUrl;

  // Get products with filters and pagination
  getProducts(filters: ProductFilters = {}): Observable<PagedResult<Product>> {
    let params = new HttpParams();
    
    // Add pagination params
    if (filters.pageNumber) params = params.set('pageNumber', filters.pageNumber.toString());
    if (filters.pageSize) params = params.set('pageSize', filters.pageSize.toString());
    if (filters.sortBy) params = params.set('sortBy', filters.sortBy);
    if (filters.sortDescending !== undefined) params = params.set('sortDescending', filters.sortDescending.toString());
    if (filters.searchTerm) params = params.set('searchTerm', filters.searchTerm);
    
    // Add filter params
    if (filters.categoryId) params = params.set('categoryId', filters.categoryId.toString());
    if (filters.subcategoryId) params = params.set('subcategoryId', filters.subcategoryId.toString());
    if (filters.brandId) params = params.set('brandId', filters.brandId.toString());

    return this.http.get<ApiResponse<PagedResult<Product>>>(`${this.apiUrl}/products`, { params })
      .pipe(
        map(response => {
          if (!response.data) {
            throw new Error('No data received');
          }
          return response.data;
        }),
        catchError(error => {
          console.error('Error fetching products:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch products'));
        })
      );
  }

  // Get product by ID
  getProductById(id: number): Observable<Product> {
    return this.http.get<ApiResponse<Product>>(`${this.apiUrl}/products/${id}`)
      .pipe(
        map(response => {
          if (!response.data) {
            throw new Error('Product not found');
          }
          return response.data;
        }),
        catchError(error => {
          console.error('Error fetching product:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch product'));
        })
      );
  }

  // Get product by slug
  getProductBySlug(slug: string): Observable<Product> {
    return this.http.get<ApiResponse<Product>>(`${this.apiUrl}/products/slug/${slug}`)
      .pipe(
        map(response => {
          if (!response.data) {
            throw new Error('Product not found');
          }
          return response.data;
        }),
        catchError(error => {
          console.error('Error fetching product:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch product'));
        })
      );
  }

  // Get featured products
  getFeaturedProducts(count: number = 10): Observable<Product[]> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<ApiResponse<Product[]>>(`${this.apiUrl}/products/featured`, { params })
      .pipe(
        map(response => response.data || []),
        catchError(error => {
          console.error('Error fetching featured products:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch featured products'));
        })
      );
  }

  // Get new products
  getNewProducts(count: number = 10): Observable<Product[]> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<ApiResponse<Product[]>>(`${this.apiUrl}/products/new`, { params })
      .pipe(
        map(response => response.data || []),
        catchError(error => {
          console.error('Error fetching new products:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch new products'));
        })
      );
  }

  // Get on sale products
  getOnSaleProducts(count: number = 10): Observable<Product[]> {
    return this.http.get<ApiResponse<Product[]>>(`${this.apiUrl}/products/on-sale?count=${count}`)
      .pipe(
        map(response => response.data || []),
        catchError(error => {
          console.error('Error fetching on-sale products:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch on-sale products'));
        })
      );
  }

  // Get best seller products
  getBestSellerProducts(count: number = 10): Observable<Product[]> {
    return this.http.get<ApiResponse<Product[]>>(`${this.apiUrl}/products/best-sellers?count=${count}`)
      .pipe(
        map(response => response.data || []),
        catchError(error => {
          console.error('Error fetching best seller products:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch best seller products'));
        })
      );
  }

  // Get popular products
  getPopularProducts(count: number = 10): Observable<Product[]> {
    return this.http.get<ApiResponse<Product[]>>(`${this.apiUrl}/products/popular?count=${count}`)
      .pipe(
        map(response => response.data || []),
        catchError(error => {
          console.error('Error fetching popular products:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch popular products'));
        })
      );
  }

  // Get related products
  getRelatedProducts(productId: number, count: number = 4): Observable<Product[]> {
    const params = new HttpParams().set('count', count.toString());
    return this.http.get<ApiResponse<Product[]>>(`${this.apiUrl}/products/${productId}/related`, { params })
      .pipe(
        map(response => response.data || []),
        catchError(error => {
          console.error('Error fetching related products:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch related products'));
        })
      );
  }

  // Search products
  searchProducts(query: string, categoryId?: string): Observable<Product[]> {
    let params = new HttpParams().set('q', query);
    if (categoryId) {
      params = params.set('category', categoryId);
    }
    
    return this.http.get<ApiResponse<Product[]>>(`${this.apiUrl}/products/search`, { params })
      .pipe(
        map(response => response.data || []),
        catchError(error => {
          console.error('Error searching products:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to search products'));
        })
      );
  }


  // Get all categories
  getCategories(): Observable<Category[]> {
    return this.http.get<ApiResponse<Category[]>>(`${this.apiUrl}/categories`).pipe(
      map(response => response.data || []),
      catchError(error => {
        console.error('Error fetching categories:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to fetch categories'));
      })
    );
  }

  getFeaturedCategories(): Observable<any[]> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/categories/featured`).pipe(
      map(response => response.data || []),
      catchError(error => {
        console.error('Error fetching featured categories:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to fetch featured categories'));
      })
    );
  }

  // Get top categories
  getTopCategories(): Observable<Category[]> {
    return this.http.get<ApiResponse<Category[]>>(`${this.apiUrl}/categories/top`)
      .pipe(
        map(response => response.data || []),
        catchError(error => {
          console.error('Error fetching top categories:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch top categories'));
        })
      );
  }

  // Get category by ID
  getCategoryById(id: number): Observable<Category> {
    return this.http.get<ApiResponse<Category>>(`${this.apiUrl}/categories/${id}`)
      .pipe(
        map(response => {
          if (!response.data) {
            throw new Error('Category not found');
          }
          return response.data;
        }),
        catchError(error => {
          console.error('Error fetching category:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch category'));
        })
      );
  }

  // Get category by slug
  getCategoryBySlug(slug: string): Observable<Category> {
    return this.http.get<ApiResponse<Category>>(`${this.apiUrl}/categories/slug/${slug}`)
      .pipe(
        map(response => {
          if (!response.data) {
            throw new Error('Category not found');
          }
          return response.data;
        }),
        catchError(error => {
          console.error('Error fetching category:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch category'));
        })
      );
  }

  // Get subcategories by category ID
  getSubcategoriesByCategoryId(categoryId: number): Observable<Subcategory[]> {
    return this.http.get<ApiResponse<Subcategory[]>>(`${this.apiUrl}/categories/${categoryId}/subcategories`)
      .pipe(
        map(response => response.data || []),
        catchError(error => {
          console.error('Error fetching subcategories:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch subcategories'));
        })
      );
  }

  // Get brands
  getBrands(): Observable<any[]> {
    return this.http.get<ApiResponse<any[]>>(`${this.apiUrl}/brands`)
      .pipe(
        map(response => response.data || []),
        catchError(error => {
          console.error('Error fetching brands:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to fetch brands'));
        })
      );
  }
}
