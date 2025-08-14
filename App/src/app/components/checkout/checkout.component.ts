import { Component, OnInit, OnDestroy, ViewChild, ElementRef, inject } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { CartService } from '../../services/cart.service';
import { AuthService } from '../../services/auth.service';
import { OrderService, CreateOrderDto } from '../../services/order.service';
import { StripeService } from '../../services/stripe.service';
import { Stripe, StripeElements, StripeCardElement } from '@stripe/stripe-js';

@Component({
  selector: 'app-checkout',
  templateUrl: './checkout.component.html',
  styleUrls: ['./checkout.component.scss']
})
export class CheckoutComponent implements OnInit, OnDestroy {
  @ViewChild('cardElement') cardElement!: ElementRef;
  
  checkoutForm!: FormGroup;
  orderNotes = '';
  showLoginAlert = false;
  showCouponAlert = false;
  selectedPaymentMethod = 'card';
  isCreatingAccount = false;
  isProcessing = false;
  paymentError = '';
  
  private fb = inject(FormBuilder);
  private cartService = inject(CartService);
  private authService = inject(AuthService);
  private orderService = inject(OrderService);
  private stripeService = inject(StripeService);
  private router = inject(Router);
  
  private destroy$ = new Subject<void>();
  private stripe: Stripe | null = null;
  private elements: StripeElements | null = null;
  private card: StripeCardElement | null = null;

  cartItems$ = this.cartService.cartItems$;
  cartSummary$ = this.cartService.cartSummary$;
  
  countries = [
    { code: 'US', name: 'United States' },
    { code: 'CA', name: 'Canada' },
    { code: 'UK', name: 'United Kingdom' },
    { code: 'AU', name: 'Australia' },
    { code: 'DE', name: 'Germany' },
    { code: 'FR', name: 'France' },
  ];

  states = [
    { code: 'WA', name: 'Washington' },
    { code: 'CA', name: 'California' },
    { code: 'NY', name: 'New York' },
    { code: 'TX', name: 'Texas' },
    { code: 'FL', name: 'Florida' },
    { code: 'IL', name: 'Illinois' },
  ];

  ngOnInit(): void {
    this.initializeForm();
    this.loadUserDataIfLoggedIn();
    this.initializeStripe();
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
    
    if (this.card) {
      this.card.destroy();
    }
  }

  async initializeStripe(): Promise<void> {
    this.stripe = await this.stripeService.getStripe();
    
    if (this.stripe) {
      this.elements = this.stripe.elements();
      
      // Wait for view to initialize
      setTimeout(() => {
        if (this.elements && this.cardElement) {
          this.card = this.elements.create('card', {
            style: {
              base: {
                fontSize: '16px',
                color: '#424770',
                '::placeholder': {
                  color: '#aab7c4',
                },
              },
              invalid: {
                color: '#9e2146',
              },
            },
          });
          
          this.card.mount(this.cardElement.nativeElement);
          
          this.card.on('change', (event) => {
            if (event.error) {
              this.paymentError = event.error.message;
            } else {
              this.paymentError = '';
            }
          });
        }
      }, 100);
    }
  }

  initializeForm(): void {
    this.checkoutForm = this.fb.group({
      firstName: ['', [Validators.required, Validators.minLength(2)]],
      lastName: ['', [Validators.required, Validators.minLength(2)]],
      companyName: [''],
      country: ['US', Validators.required],
      streetAddress: ['', Validators.required],
      apartmentSuite: [''],
      city: ['', Validators.required],
      state: ['WA', Validators.required],
      zipCode: ['', [Validators.required, Validators.minLength(3)]], // More flexible validation
      phoneNumber: ['', [Validators.required, Validators.pattern(/^\+?[\d\s-()]+$/)]],
      email: ['', [Validators.required, Validators.email]],
    });
  }

  loadUserDataIfLoggedIn(): void {
    const currentUser = this.authService.currentUser;
    if (currentUser) {
      const nameParts = currentUser.name.split(' ');
      this.checkoutForm.patchValue({
        firstName: nameParts[0] || '',
        lastName: nameParts.slice(1).join(' ') || '',
        email: currentUser.email
      });
    }
  }

  toggleLoginAlert(): void {
    this.showLoginAlert = !this.showLoginAlert;
  }

  toggleCouponAlert(): void {
    this.showCouponAlert = !this.showCouponAlert;
  }

  selectPaymentMethod(method: string): void {
    this.selectedPaymentMethod = method;
  }

  getShippingCost(): number {
    const cartSummary = this.cartService.cartSummary;
    return cartSummary.shippingEstimate || 0;
  }

  getTax(): number {
    const cartSummary = this.cartService.cartSummary;
    return cartSummary.taxEstimate || (cartSummary.subtotal * 0.08); // 8% tax
  }

  getOrderTotal(): number {
    const cartSummary = this.cartService.cartSummary;
    return cartSummary.subtotal + this.getShippingCost() + this.getTax();
  }

  async placeOrder(): Promise<void> {
    if (!this.checkoutForm.valid) {
      Object.keys(this.checkoutForm.controls).forEach(key => {
        this.checkoutForm.get(key)?.markAsTouched();
      });
      return;
    }

    this.isProcessing = true;
    this.paymentError = '';

    try {
      const formValue = this.checkoutForm.value;
      let paymentIntentId: string | undefined;

      // Handle Stripe payment if card method is selected
      if (this.selectedPaymentMethod === 'card' && this.stripe && this.card) {
        // Create payment intent
        const total = this.getOrderTotal();
        const paymentIntent = await this.orderService.createPaymentIntent(total).toPromise();
        
        if (!paymentIntent?.clientSecret) {
          throw new Error('Failed to create payment intent');
        }

        // Confirm card payment
        const { error, paymentIntent: confirmedPayment } = await this.stripe.confirmCardPayment(
          paymentIntent.clientSecret,
          {
            payment_method: {
              card: this.card,
              billing_details: {
                name: `${formValue.firstName} ${formValue.lastName}`,
                email: formValue.email,
                phone: formValue.phoneNumber,
                address: {
                  line1: formValue.streetAddress,
                  line2: formValue.apartmentSuite,
                  city: formValue.city,
                  state: formValue.state,
                  postal_code: formValue.zipCode,
                  country: formValue.country.toLowerCase(), // Stripe expects lowercase country codes
                },
              },
            },
          }
        );

        if (error) {
          throw new Error(error.message);
        }

        paymentIntentId = confirmedPayment?.id;
      }

      // Create order
      const cartItems = this.cartService.cartItems;
      const orderData: CreateOrderDto = {
        firstName: formValue.firstName,
        lastName: formValue.lastName,
        companyName: formValue.companyName,
        email: formValue.email,
        phoneNumber: formValue.phoneNumber,
        streetAddress: formValue.streetAddress,
        apartmentSuite: formValue.apartmentSuite,
        city: formValue.city,
        state: formValue.state,
        country: formValue.country,
        zipCode: formValue.zipCode,
        orderNotes: this.orderNotes,
        paymentMethod: this.selectedPaymentMethod,
        paymentIntentId: paymentIntentId,
        createAccount: this.isCreatingAccount,
        password: this.isCreatingAccount ? 'tempPassword123' : undefined, // You should add a password field if createAccount is true
        cartItems: cartItems.map(item => ({
          productId: item.id,
          productName: item.name,
          productImage: item.image,
          quantity: item.quantity,
          price: item.price,
          shipping: item.shipping,
          shippingCost: item.shippingCost
        }))
      };

      const order = await this.orderService.createOrder(orderData).toPromise();

      if (order) {
        // Clear cart
        this.cartService.clearCart();
        
        // Navigate to order success page
        this.router.navigate(['/order-success', order.orderNumber]);
      }
    } catch (error: any) {
      console.error('Order error:', error);
      this.paymentError = error.message || 'An error occurred while processing your order';
    } finally {
      this.isProcessing = false;
    }
  }
} 