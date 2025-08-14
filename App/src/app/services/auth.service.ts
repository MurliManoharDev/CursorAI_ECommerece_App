import { Injectable, inject } from '@angular/core';
import { BehaviorSubject, Observable, throwError } from 'rxjs';
import { Router } from '@angular/router';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { map, catchError } from 'rxjs/operators';
import { environment } from '../../environments/environment';

export interface User {
  id: string;
  email: string;
  name: string;
}

interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  errors?: string[];
}

interface AuthResponse {
  token: string;
  expiresAt: Date;
  user: {
    id: number;
    email: string;
    firstName?: string;
    lastName?: string;
  };
}

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private isLoggedInSubject: BehaviorSubject<boolean>;
  private currentUserSubject: BehaviorSubject<User | null>;
  
  public isLoggedIn$: Observable<boolean>;
  public currentUser$: Observable<User | null>;

  private router = inject(Router);
  private http = inject(HttpClient);
  private apiUrl = `${environment.apiUrl}/auth`;

  constructor() {
    // Check if user data exists in localStorage on initialization
    const userToken = localStorage.getItem('userToken');
    const userData = localStorage.getItem('userData');
    
    const isLoggedIn = !!(userToken && userData);
    const user = isLoggedIn && userData ? JSON.parse(userData) : null;
    
    this.isLoggedInSubject = new BehaviorSubject<boolean>(isLoggedIn);
    this.currentUserSubject = new BehaviorSubject<User | null>(user);
    
    this.isLoggedIn$ = this.isLoggedInSubject.asObservable();
    this.currentUser$ = this.currentUserSubject.asObservable();
  }

  get isLoggedIn(): boolean {
    return this.isLoggedInSubject.value;
  }

  get currentUser(): User | null {
    return this.currentUserSubject.value;
  }

  getToken(): string | null {
    return localStorage.getItem('userToken');
  }

  login(email: string, password: string): Observable<boolean> {
    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/login`, { email, password })
      .pipe(
        map(response => {
          if (response.success && response.data) {
            const authData = response.data;
            
            // Store token and user data
            localStorage.setItem('userToken', authData.token);
            const userData: User = {
              id: authData.user.id.toString(),
              email: authData.user.email,
              name: `${authData.user.firstName || ''} ${authData.user.lastName || ''}`.trim() || authData.user.email.split('@')[0]
            };
            localStorage.setItem('userData', JSON.stringify(userData));

            // Update subjects
            this.currentUserSubject.next(userData);
            this.isLoggedInSubject.next(true);

            return true;
          }
          return false;
        }),
        catchError(error => {
          console.error('Login error:', error);
          return throwError(() => new Error(error.error?.message || 'Login failed'));
        })
      );
  }

  register(firstName: string, lastName: string, email: string, phone: string, password: string): Observable<boolean> {
    const registerData = {
      email,
      password,
      firstName,
      lastName,
      phone
    };

    return this.http.post<ApiResponse<AuthResponse>>(`${this.apiUrl}/register`, registerData)
      .pipe(
        map(response => {
          if (response.success && response.data) {
            const authData = response.data;
            
            // Store token and user data
            localStorage.setItem('userToken', authData.token);
            const userData: User = {
              id: authData.user.id.toString(),
              email: authData.user.email,
              name: `${firstName} ${lastName}`.trim()
            };
            localStorage.setItem('userData', JSON.stringify(userData));

            // Update subjects
            this.currentUserSubject.next(userData);
            this.isLoggedInSubject.next(true);

            return true;
          }
          return false;
        }),
        catchError(error => {
          console.error('Registration error:', error);
          if (error.error?.message === 'Email already exists') {
            return throwError(() => new Error('Email already exists'));
          }
          return throwError(() => new Error(error.error?.message || 'Registration failed'));
        })
      );
  }

  logout(): void {
    // Clear localStorage
    localStorage.removeItem('userToken');
    localStorage.removeItem('userData');
    
    // Update subjects
    this.currentUserSubject.next(null);
    this.isLoggedInSubject.next(false);
    
    // Navigate to home
    this.router.navigate(['/']);
  }

  // Helper method to get auth headers
  getAuthHeaders(): HttpHeaders {
    const token = localStorage.getItem('userToken');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
  }

  changePassword(currentPassword: string, newPassword: string): Observable<boolean> {
    const token = this.getToken();
    
    if (!token) {
      return throwError(() => new Error('User not authenticated'));
    }
    
    const headers = new HttpHeaders({
      'Content-Type': 'application/json',
      'Authorization': `Bearer ${token}`
    });
    
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/change-password`, {
      currentPassword,
      newPassword
    }, { headers })
      .pipe(
        map(response => {
          if (response.success) {
            return true;
          }
          throw new Error(response.message || 'Failed to change password');
        }),
        catchError(error => {
          console.error('Change password error:', error);
          return throwError(() => new Error(error.error?.message || 'Current password is incorrect'));
        })
      );
  }

  forgotPassword(email: string): Observable<boolean> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/forgot-password`, { email })
      .pipe(
        map(response => {
          if (response.success) {
            return true;
          }
          throw new Error(response.message || 'Failed to send reset email');
        }),
        catchError(error => {
          console.error('Forgot password error:', error);
          return throwError(() => new Error(error.error?.message || 'Failed to send reset email'));
        })
      );
  }

  resetPassword(email: string, token: string, newPassword: string): Observable<boolean> {
    return this.http.post<ApiResponse<any>>(`${this.apiUrl}/reset-password`, {
      email,
      token,
      newPassword
    })
      .pipe(
        map(response => {
          if (response.success) {
            return true;
          }
          throw new Error(response.message || 'Failed to reset password');
        }),
        catchError(error => {
          console.error('Reset password error:', error);
          return throwError(() => new Error(error.error?.message || 'Invalid or expired reset token'));
        })
      );
  }
}
