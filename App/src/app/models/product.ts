export interface Product {
  id?: number;
  name: string;
  subtitle?: string;
  imageUrl: string;
  price: number;
  oldPrice?: number;
  rating: number;
  reviews: number;
  inStock: boolean;
  tags?: string[];
  new?: boolean;
  description?: string;
  freeShipping?: boolean;
  shippingCost?: number;
  freeGift?: boolean;
  savings?: number;
  contactForPrice?: boolean;
  variants?: ProductVariant[];
  preOrder?: boolean;
  originalPrice?: number;
  isOnSale?: boolean;
  isNew?: boolean;
  reviewCount?: number;
  priceRange?: {
    min: number;
    max: number;
  };
  image?: string; // legacy support
}

export interface ProductVariant {
  imageUrl: string;
  color?: string;
}

export interface Category {
  id: number;
  name: string;
  imageUrl: string;
  itemCount: number;
}

export interface PromotionalItem {
  id: number;
  title: string;
  subtitle: string;
  description: string;
  imageUrl: string;
  buttonText: string;
  buttonLink: string;
}