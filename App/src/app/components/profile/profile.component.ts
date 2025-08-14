import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors } from '@angular/forms';
import { AuthService, User } from '../../services/auth.service';
import { AddressService, UserAddress, CreateAddressDto } from '../../services/address.service';

@Component({
  selector: 'app-profile',
  templateUrl: './profile.component.html',
  styleUrls: ['./profile.component.scss']
})
export class ProfileComponent implements OnInit {
  profileForm!: FormGroup;
  changePasswordForm!: FormGroup;
  addressForm!: FormGroup;
  activeTab = 'account-info';
  isLoading = false;
  isChangingPassword = false;
  isSavingAddress = false;
  isDeletingAddress = false;
  successMessage = '';
  errorMessage = '';
  changePasswordSuccess = '';
  changePasswordError = '';
  addressSuccess = '';
  addressError = '';
  userData: User | null = null;
  showCurrentPassword = false;
  showNewPassword = false;
  showConfirmPassword = false;
  
  // Address related properties
  addresses: UserAddress[] = [];
  showAddressForm = false;
  editingAddressId: number | null = null;
  deletingAddressId: number | null = null;

  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private addressService = inject(AddressService);

  ngOnInit(): void {
    this.initializeForm();
    this.initializeChangePasswordForm();
    this.initializeAddressForm();
    this.loadUserData();
  }

  initializeForm(): void {
    this.profileForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.pattern(/^\+?[\d\s-()]+$/)]]
    });
  }

  initializeChangePasswordForm(): void {
    this.changePasswordForm = this.fb.group({
      currentPassword: ['', [Validators.required]],
      newPassword: ['', [
        Validators.required,
        Validators.minLength(6),
        Validators.pattern(/^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*#?&]{6,}$/)
      ]],
      confirmPassword: ['', [Validators.required]]
    }, { validators: this.passwordMatchValidator });
  }

  initializeAddressForm(): void {
    this.addressForm = this.fb.group({
      addressType: ['shipping', [Validators.required]],
      addressLine1: ['', [Validators.required, Validators.maxLength(255)]],
      addressLine2: ['', [Validators.maxLength(255)]],
      city: ['', [Validators.required, Validators.maxLength(100)]],
      stateProvince: ['', [Validators.maxLength(100)]],
      postalCode: ['', [Validators.maxLength(20)]],
      country: ['', [Validators.required, Validators.maxLength(100)]],
      isDefault: [false]
    });
  }

  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const newPassword = control.get('newPassword');
    const confirmPassword = control.get('confirmPassword');

    if (!newPassword || !confirmPassword) {
      return null;
    }

    if (newPassword.value !== confirmPassword.value) {
      return { passwordMismatch: true };
    }

    return null;
  }

  loadUserData(): void {
    // Load user data from AuthService
    const currentUser = this.authService.currentUser;
    if (currentUser) {
      this.userData = currentUser;
      // Parse name into first and last name
      const nameParts = currentUser.name.split(' ');
      this.profileForm.patchValue({
        firstName: nameParts[0] || '',
        lastName: nameParts.slice(1).join(' ') || '',
        email: currentUser.email,
        phoneNumber: '+1 0231 4554 452' // Mock phone number
      });
    }
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
    // Clear messages when switching tabs
    this.successMessage = '';
    this.errorMessage = '';
    this.changePasswordSuccess = '';
    this.changePasswordError = '';
    this.addressSuccess = '';
    this.addressError = '';
    
    // Load addresses when switching to address tab
    if (tab === 'my-address') {
      this.loadAddresses();
    }
  }

  onSubmit(): void {
    if (this.profileForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      // Simulate API call to update profile
      setTimeout(() => {
        this.isLoading = false;
        this.successMessage = 'Profile updated successfully!';
        
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.successMessage = '';
        }, 3000);
      }, 1000);
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.profileForm.controls).forEach(key => {
        this.profileForm.get(key)?.markAsTouched();
      });
    }
  }

  onChangePassword(): void {
    if (this.changePasswordForm.valid) {
      this.isChangingPassword = true;
      this.changePasswordError = '';
      this.changePasswordSuccess = '';

      const { currentPassword, newPassword } = this.changePasswordForm.value;

      this.authService.changePassword(currentPassword, newPassword).subscribe({
        next: (success: boolean) => {
          if (success) {
            this.changePasswordSuccess = 'Password changed successfully!';
            this.isChangingPassword = false;
            this.changePasswordForm.reset();
            
            // Clear success message after 3 seconds
            setTimeout(() => {
              this.changePasswordSuccess = '';
            }, 3000);
          }
        },
        error: (error: Error) => {
          console.error('Password change failed:', error);
          this.changePasswordError = error.message || 'Failed to change password. Please try again.';
          this.isChangingPassword = false;
        }
      });
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.changePasswordForm.controls).forEach(key => {
        this.changePasswordForm.get(key)?.markAsTouched();
      });
    }
  }

  togglePasswordVisibility(field: 'current' | 'new' | 'confirm'): void {
    switch (field) {
      case 'current':
        this.showCurrentPassword = !this.showCurrentPassword;
        break;
      case 'new':
        this.showNewPassword = !this.showNewPassword;
        break;
      case 'confirm':
        this.showConfirmPassword = !this.showConfirmPassword;
        break;
    }
  }

  // Address methods
  loadAddresses(): void {
    this.addressService.getAddresses().subscribe({
      next: (addresses) => {
        this.addresses = addresses;
      },
      error: (error) => {
        console.error('Failed to load addresses:', error);
        this.addressError = 'Failed to load addresses. Please try again.';
      }
    });
  }

  showAddAddress(): void {
    this.showAddressForm = true;
    this.editingAddressId = null;
    this.addressForm.reset({
      addressType: 'shipping',
      isDefault: false
    });
  }

  editAddress(address: UserAddress): void {
    this.showAddressForm = true;
    this.editingAddressId = address.id;
    this.addressForm.patchValue({
      addressType: address.addressType,
      addressLine1: address.addressLine1,
      addressLine2: address.addressLine2 || '',
      city: address.city,
      stateProvince: address.stateProvince || '',
      postalCode: address.postalCode || '',
      country: address.country,
      isDefault: address.isDefault
    });
  }

  cancelAddressForm(): void {
    this.showAddressForm = false;
    this.editingAddressId = null;
    this.addressForm.reset();
    this.addressError = '';
    this.addressSuccess = '';
  }

  onSaveAddress(): void {
    if (this.addressForm.valid) {
      this.isSavingAddress = true;
      this.addressError = '';
      this.addressSuccess = '';

      const addressData: CreateAddressDto = this.addressForm.value;

      const saveObservable = this.editingAddressId
        ? this.addressService.updateAddress(this.editingAddressId, addressData)
        : this.addressService.createAddress(addressData);

      saveObservable.subscribe({
        next: (address) => {
          this.addressSuccess = this.editingAddressId 
            ? 'Address updated successfully!' 
            : 'Address added successfully!';
          this.isSavingAddress = false;
          this.showAddressForm = false;
          this.editingAddressId = null;
          this.addressForm.reset();
          this.loadAddresses();
          
          // Clear success message after 3 seconds
          setTimeout(() => {
            this.addressSuccess = '';
          }, 3000);
        },
        error: (error) => {
          console.error('Failed to save address:', error);
          this.addressError = error.message || 'Failed to save address. Please try again.';
          this.isSavingAddress = false;
        }
      });
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.addressForm.controls).forEach(key => {
        this.addressForm.get(key)?.markAsTouched();
      });
    }
  }

  confirmDeleteAddress(addressId: number): void {
    this.deletingAddressId = addressId;
  }

  cancelDelete(): void {
    this.deletingAddressId = null;
  }

  deleteAddress(addressId: number): void {
    this.isDeletingAddress = true;
    this.addressError = '';
    
    this.addressService.deleteAddress(addressId).subscribe({
      next: () => {
        this.addressSuccess = 'Address deleted successfully!';
        this.isDeletingAddress = false;
        this.deletingAddressId = null;
        this.loadAddresses();
        
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.addressSuccess = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Failed to delete address:', error);
        this.addressError = error.message || 'Failed to delete address. Please try again.';
        this.isDeletingAddress = false;
        this.deletingAddressId = null;
      }
    });
  }

  setAsDefault(addressId: number): void {
    this.addressService.setDefaultAddress(addressId).subscribe({
      next: () => {
        this.addressSuccess = 'Default address updated successfully!';
        this.loadAddresses();
        
        // Clear success message after 3 seconds
        setTimeout(() => {
          this.addressSuccess = '';
        }, 3000);
      },
      error: (error) => {
        console.error('Failed to set default address:', error);
        this.addressError = error.message || 'Failed to set default address. Please try again.';
      }
    });
  }

  getAvatarUrl(): string {
    // Return avatar URL or placeholder
    // Using UI Avatars service for placeholder avatar
    const name = this.userData?.name || 'Mark Cole';
    return `https://ui-avatars.com/api/?name=${encodeURIComponent(name)}&size=220&background=1ABA1A&color=fff&bold=true`;
  }
}
