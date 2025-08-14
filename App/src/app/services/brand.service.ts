import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface Brand {
  id: number;
  name: string;
  logoUrl?: string;
  description?: string;
  isFeatured: boolean;
  displayOrder: number;
  productCount: number;
}

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data: T;
}

@Injectable({
  providedIn: 'root'
})
export class BrandService {
  private apiUrl = `${environment.apiUrl}/brands`;

  constructor(private http: HttpClient) { }

  // Get all brands
  getAllBrands(): Observable<Brand[]> {
    return this.http.get<ApiResponse<Brand[]>>(this.apiUrl).pipe(
      map(response => response.data || []),
      catchError(error => {
        console.error('Error fetching brands:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to fetch brands'));
      })
    );
  }

  // Get featured brands
  getFeaturedBrands(): Observable<Brand[]> {
    return this.http.get<ApiResponse<Brand[]>>(`${this.apiUrl}/featured`).pipe(
      map(response => response.data || []),
      catchError(error => {
        console.error('Error fetching featured brands:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to fetch featured brands'));
      })
    );
  }

  // Get brand by ID
  getBrandById(id: number): Observable<Brand> {
    return this.http.get<ApiResponse<Brand>>(`${this.apiUrl}/${id}`).pipe(
      map(response => response.data),
      catchError(error => {
        console.error('Error fetching brand:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to fetch brand'));
      })
    );
  }
} 