import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.scss']
})
export class LoginComponent implements OnInit {
  loginForm!: FormGroup;
  showPassword = false;
  isLoading = false;
  errorMessage = '';
  successMessage = '';

  private fb = inject(FormBuilder);
  private router = inject(Router);
  private authService = inject(AuthService);

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required]]
    });
  }

  togglePassword(): void {
    this.showPassword = !this.showPassword;
  }

  onSubmit(): void {
    if (this.loginForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';
      
      const { email, password } = this.loginForm.value;
      
      this.authService.login(email, password).subscribe({
        next: (success) => {
          if (success) {
            this.successMessage = 'Login successful! Redirecting...';
            this.isLoading = false;
            setTimeout(() => {
              this.router.navigate(['/']);
            }, 1000);
          } else {
            this.errorMessage = 'Login failed. Please try again.';
            this.isLoading = false;
          }
        },
        error: (error) => {
          console.error('Login failed:', error);
          // Handle specific error messages from the API
          if (error.message === 'Invalid email or password') {
            this.errorMessage = 'Invalid email or password. Please check your credentials.';
          } else if (error.message.includes('network')) {
            this.errorMessage = 'Network error. Please check your connection and try again.';
          } else {
            this.errorMessage = error.message || 'Login failed. Please try again later.';
          }
          this.isLoading = false;
        }
      });
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.loginForm.controls).forEach(key => {
        this.loginForm.get(key)?.markAsTouched();
      });
    }
  }
}
