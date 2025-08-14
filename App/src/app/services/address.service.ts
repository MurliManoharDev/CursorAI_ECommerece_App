import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

export interface UserAddress {
  id: number;
  addressType: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  stateProvince?: string;
  postalCode?: string;
  country: string;
  isDefault: boolean;
}

export interface CreateAddressDto {
  addressType: string;
  addressLine1: string;
  addressLine2?: string;
  city: string;
  stateProvince?: string;
  postalCode?: string;
  country: string;
  isDefault: boolean;
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
export class AddressService {
  private http = inject(HttpClient);
  private authService = inject(AuthService);
  private apiUrl = `${environment.apiUrl}/users/addresses`;

  getAddresses(): Observable<UserAddress[]> {
    return this.http.get<ApiResponse<UserAddress[]>>(this.apiUrl, {
      headers: this.authService.getAuthHeaders()
    }).pipe(
      map(response => response.data || []),
      catchError(error => {
        console.error('Error fetching addresses:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to fetch addresses'));
      })
    );
  }

  getAddressById(addressId: number): Observable<UserAddress> {
    return this.http.get<ApiResponse<UserAddress>>(`${this.apiUrl}/${addressId}`, {
      headers: this.authService.getAuthHeaders()
    }).pipe(
      map(response => {
        if (!response.data) {
          throw new Error('Address not found');
        }
        return response.data;
      }),
      catchError(error => {
        console.error('Error fetching address:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to fetch address'));
      })
    );
  }

  createAddress(address: CreateAddressDto): Observable<UserAddress> {
    return this.http.post<ApiResponse<UserAddress>>(this.apiUrl, address, {
      headers: this.authService.getAuthHeaders()
    }).pipe(
      map(response => {
        if (!response.data) {
          throw new Error('Failed to create address');
        }
        return response.data;
      }),
      catchError(error => {
        console.error('Error creating address:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to create address'));
      })
    );
  }

  updateAddress(addressId: number, address: CreateAddressDto): Observable<UserAddress> {
    return this.http.put<ApiResponse<UserAddress>>(`${this.apiUrl}/${addressId}`, address, {
      headers: this.authService.getAuthHeaders()
    }).pipe(
      map(response => {
        if (!response.data) {
          throw new Error('Failed to update address');
        }
        return response.data;
      }),
      catchError(error => {
        console.error('Error updating address:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to update address'));
      })
    );
  }

  deleteAddress(addressId: number): Observable<boolean> {
    return this.http.delete<ApiResponse<void>>(`${this.apiUrl}/${addressId}`, {
      headers: this.authService.getAuthHeaders()
    }).pipe(
      map(response => response.success),
      catchError(error => {
        console.error('Error deleting address:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to delete address'));
      })
    );
  }

  setDefaultAddress(addressId: number): Observable<boolean> {
    return this.http.put<ApiResponse<void>>(`${this.apiUrl}/${addressId}/default`, {}, {
      headers: this.authService.getAuthHeaders()
    }).pipe(
      map(response => response.success),
      catchError(error => {
        console.error('Error setting default address:', error);
        return throwError(() => new Error(error.error?.message || 'Failed to set default address'));
      })
    );
  }
} 