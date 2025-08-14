import { Injectable } from '@angular/core';
import { loadStripe, Stripe, StripeElements, StripeCardElement } from '@stripe/stripe-js';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class StripeService {
  private stripePromise: Promise<Stripe | null>;

  constructor() {
    // TODO: Replace with your actual Stripe publishable key from https://dashboard.stripe.com/test/apikeys
    // Example: 'pk_test_51ABC...XYZ'
    const STRIPE_PUBLISHABLE_KEY = 'pk_test_51RvhynK6DQALz7V87QO3HiMMI2gBHfeaM67GHSeBMixRExRehoDE9nORT0QTZyuYXRcyLHfnAkqt3kN2WIZD6ALT00UEpag5PT';
    
    this.stripePromise = loadStripe(STRIPE_PUBLISHABLE_KEY);
  }

  async getStripe(): Promise<Stripe | null> {
    return await this.stripePromise;
  }

  async createElement(elements: StripeElements): Promise<StripeCardElement> {
    const cardElement = elements.create('card', {
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
    
    return cardElement;
  }
} 