import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { Router, ActivatedRoute } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {
  resetPasswordForm!: FormGroup;
  isLoading = false;
  errorMessage = '';
  successMessage = '';
  showNewPassword = false;
  showConfirmPassword = false;
  
  token: string = '';
  email: string = '';
  isValidToken = false;
  checkingToken = true;

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);
  private route = inject(ActivatedRoute);

  ngOnInit(): void {
    // Get token and email from URL parameters
    this.route.queryParams.subscribe(params => {
      this.token = params['token'] || '';
      this.email = params['email'] || '';
      
      if (!this.token || !this.email) {
        this.checkingToken = false;
        this.errorMessage = 'Invalid reset link. Please request a new password reset.';
        return;
      }
      
      // Token is present, assume it's valid for now
      // In a real app, you might want to verify the token with the backend
      this.isValidToken = true;
      this.checkingToken = false;
      this.initializeForm();
    });
  }

  initializeForm(): void {
    this.resetPasswordForm = this.fb.group({
      newPassword: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(/^(?=.*[A-Za-z])(?=.*\d).+$/) // At least one letter and one number
      ]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const newPassword = control.get('newPassword');
    const confirmPassword = control.get('confirmPassword');

    if (!newPassword || !confirmPassword) {
      return null;
    }

    if (confirmPassword.value === '') {
      return null;
    }

    if (newPassword.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }

    return null;
  }

  toggleNewPasswordVisibility(): void {
    this.showNewPassword = !this.showNewPassword;
  }

  toggleConfirmPasswordVisibility(): void {
    this.showConfirmPassword = !this.showConfirmPassword;
  }

  onSubmit(): void {
    if (this.resetPasswordForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const { newPassword } = this.resetPasswordForm.value;

      this.authService.resetPassword(this.email, this.token, newPassword).subscribe({
        next: () => {
          this.isLoading = false;
          this.successMessage = 'Your password has been reset successfully!';
          this.resetPasswordForm.reset();
          
          // Redirect to login after 3 seconds
          setTimeout(() => {
            this.router.navigate(['/login']);
          }, 3000);
        },
        error: (error) => {
          this.isLoading = false;
          if (error.message === 'Invalid or expired reset token') {
            this.errorMessage = 'Your reset link has expired or is invalid. Please request a new one.';
          } else {
            this.errorMessage = error.message || 'Failed to reset password. Please try again.';
          }
        }
      });
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.resetPasswordForm.controls).forEach(key => {
        this.resetPasswordForm.get(key)?.markAsTouched();
      });
    }
  }

  navigateToForgotPassword(): void {
    this.router.navigate(['/forgot-password']);
  }

  navigateToLogin(): void {
    this.router.navigate(['/login']);
  }

  // Helper methods for password validation
  hasMinLength(): boolean {
    const password = this.resetPasswordForm?.get('newPassword')?.value || '';
    return password.length >= 6;
  }

  hasLetter(): boolean {
    const password = this.resetPasswordForm?.get('newPassword')?.value || '';
    return /[A-Za-z]/.test(password);
  }

  hasNumber(): boolean {
    const password = this.resetPasswordForm?.get('newPassword')?.value || '';
    return /\d/.test(password);
  }
}
