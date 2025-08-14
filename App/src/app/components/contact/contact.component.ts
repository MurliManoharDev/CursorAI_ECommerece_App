import { Component, OnInit, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

interface Country {
  code: string;
  name: string;
}

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss']
})
export class ContactComponent implements OnInit {
  contactForm!: FormGroup;
  isLoading = false;
  successMessage = '';
  errorMessage = '';
  newsletterConsent = false;
  
  private fb = inject(FormBuilder);

  // Countries list
  countries: Country[] = [
    { code: 'US', name: 'United States (US)' },
    { code: 'UK', name: 'United Kingdom (UK)' },
    { code: 'CA', name: 'Canada' },
    { code: 'AU', name: 'Australia' },
    { code: 'DE', name: 'Germany' },
    { code: 'FR', name: 'France' },
    { code: 'IT', name: 'Italy' },
    { code: 'ES', name: 'Spain' },
    { code: 'IN', name: 'India' },
    { code: 'CN', name: 'China' },
    { code: 'JP', name: 'Japan' },
    { code: 'KR', name: 'South Korea' }
  ];

  // Contact information
  contactInfo = {
    us: {
      title: 'united states (head quater)',
      address: '152 Thatcher Road St, Mahattan, 10463, US',
      phone: '(+025) 3886 25 16',
      email: 'hello@swattechmart.com'
    },
    uk: {
      title: 'united kingdom (branch)',
      address: '12 Buckingham Rd, Thornthwaite, HG3 4TY, UK',
      phone: '(+718) 895-5350',
      email: 'contact@swattechmart.co.uk'
    }
  };

  // Social media links
  socialLinks = [
    { icon: 'fab fa-facebook-f', url: 'https://facebook.com', label: 'Facebook' },
    { icon: 'fab fa-twitter', url: 'https://twitter.com', label: 'Twitter' },
    { icon: 'fab fa-instagram', url: 'https://instagram.com', label: 'Instagram' },
    { icon: 'fab fa-linkedin-in', url: 'https://linkedin.com', label: 'LinkedIn' },
    { icon: 'fab fa-youtube', url: 'https://youtube.com', label: 'YouTube' }
  ];

  ngOnInit(): void {
    this.initializeForm();
  }

  initializeForm(): void {
    this.contactForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      email: ['', [Validators.required, Validators.email]],
      phoneNumber: ['', [Validators.pattern(/^\+?[\d\s-()]+$/)]],
      country: ['US', [Validators.required]],
      subject: [''],
      message: ['', [Validators.required, Validators.minLength(10)]]
    });
  }

  onSubmit(): void {
    if (this.contactForm.valid) {
      this.isLoading = true;
      this.errorMessage = '';
      this.successMessage = '';

      const formData = {
        ...this.contactForm.value,
        newsletterConsent: this.newsletterConsent
      };

      // Simulate API call to send contact form
      setTimeout(() => {
        console.log('Form submitted:', formData);
        this.isLoading = false;
        this.successMessage = 'Thank you for your message! We\'ll get back to you soon.';
        this.contactForm.reset({ country: 'US' });
        this.newsletterConsent = false;
        
        // Clear success message after 5 seconds
        setTimeout(() => {
          this.successMessage = '';
        }, 5000);
      }, 1500);
    } else {
      // Mark all fields as touched to show validation errors
      Object.keys(this.contactForm.controls).forEach(key => {
        this.contactForm.get(key)?.markAsTouched();
      });
    }
  }

  toggleNewsletterConsent(): void {
    this.newsletterConsent = !this.newsletterConsent;
  }
}
