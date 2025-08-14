import { Component, OnInit, inject, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CartService, CartItem } from '../../services/cart.service';
import { WishlistService } from '../../services/wishlist.service';
import { ProductService, Product as ApiProduct } from '../../services/product.service';
import { Subject, takeUntil } from 'rxjs';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../../environments/environment';

interface ProductDetail {
  id: number;
  name: string;
  price: number;
  priceRange?: { min: number; max: number };
  rating: number;
  reviews: number;
  category: string;
  categoryBreadcrumb?: string[];
  sku: string;
  availability?: string;
  tags?: string[];
  features?: string[];
  description: string;
  images?: string[];
  currentImage?: string;
  selectedOptions?: { color?: string; storage?: string };
  specifications?: Record<string, unknown>;
  additionalInfo?: Record<string, unknown>;
  warranty?: string;
  shipping?: string;
  brand?: string;
  inStock?: boolean;
  freeShipping?: boolean;
  hasGift?: boolean;
  isNew?: boolean;
}

interface ProductImage {
  id: number;
  url: string;
  isMain: boolean;
}

interface ProductVariant {
  id: string;
  type: string; // 'color' or 'size'
  name: string;
  value: string;
  price: number;
  image?: string;
  inStock: boolean;
}

interface RelatedProduct {
  id: number;
  name: string;
  price: number;
  originalPrice?: number;
  image: string;
  rating: number;
  reviews: number;
  freeShipping?: boolean;
  discount?: number;
  isNew?: boolean;
}

interface FrequentlyBoughtProduct {
  id: number;
  name: string;
  price: number;
  oldPrice?: number;
  imageUrl: string;
  selected: boolean;
}

@Component({
  selector: 'app-product-detail',
  templateUrl: './product-detail.component.html',
  styleUrls: ['./product-detail.component.scss']
})
export class ProductDetailComponent implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();
  isLoading = true;
  errorMessage = '';

  product: ProductDetail = {
    id: 0,
    name: '',
    price: 0,
    rating: 0,
    reviews: 0,
    sku: '',
    category: '',
    brand: '',
    inStock: true,
    freeShipping: false,
    hasGift: false,
    description: ''
  };

  // Product images
  productImages: ProductImage[] = [];
  selectedImage: ProductImage = { id: 1, url: 'assets/images/prod1.png', isMain: true };

  // Product features
  productFeatures: string[] = [];

  // Product variants
  colorVariants: ProductVariant[] = [];
  memoryVariants: ProductVariant[] = [];
  selectedColor: ProductVariant | null = null;
  selectedMemory: ProductVariant | null = null;

  // Quantity
  quantity = 1;

  // Tabs
  activeTab = 'description';
  isInWishlist = false;

  // Frequently bought together
  frequentlyBoughtProducts: FrequentlyBoughtProduct[] = [];

  // Related products
  relatedProducts: RelatedProduct[] = [];

  // Recently viewed
  recentlyViewedProducts: any[] = [];

  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private cartService = inject(CartService);
  private wishlistService = inject(WishlistService);
  private productService = inject(ProductService);
  private http = inject(HttpClient);

  ngOnInit(): void {
    // Get product ID or slug from route
    this.route.params.pipe(takeUntil(this.destroy$)).subscribe(params => {
      const productId = params['id'];
      const productSlug = params['slug'];
      
      console.log('Route params:', { productId, productSlug });
      
      if (productId && !isNaN(+productId)) {
        // If we have a numeric ID, load by ID
        this.loadProduct(+productId);
      } else if (productId && isNaN(+productId)) {
        // If the ID parameter is actually a slug (non-numeric), load by slug
        this.loadProductBySlug(productId);
      } else if (productSlug) {
        // If we have a slug parameter, load by slug
        this.loadProductBySlug(productSlug);
      } else {
        console.error('No valid product identifier found');
        this.errorMessage = 'Invalid product identifier';
        this.isLoading = false;
      }
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  private loadProduct(productId: number): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.productService.getProductById(productId)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (apiProduct: any) => {
          this.mapProductData(apiProduct);
          this.loadRelatedProducts(productId);
          this.loadRecentlyViewed();
          this.loadFrequentlyBoughtProducts();
          this.checkWishlistStatus();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading product:', error);
          this.errorMessage = 'Failed to load product details';
          this.isLoading = false;
        }
      });
  }

  private loadProductBySlug(slug: string): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.productService.getProductBySlug(slug)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (apiProduct: any) => {
          this.mapProductData(apiProduct);
          this.loadRelatedProducts(apiProduct.id);
          this.loadRecentlyViewed();
          this.loadFrequentlyBoughtProducts();
          this.checkWishlistStatus();
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Error loading product by slug:', error);
          this.errorMessage = 'Failed to load product details';
          this.isLoading = false;
        }
      });
  }

  private mapProductData(apiProduct: any): void {
    // Map basic product data
    this.product = {
      id: apiProduct.id,
      name: apiProduct.name,
      price: apiProduct.price,
      rating: apiProduct.averageRating || 0,
      reviews: apiProduct.reviewCount || 0,
      sku: apiProduct.sku || 'N/A',
      category: apiProduct.categoryName,
      brand: apiProduct.brandName || 'Generic',
      inStock: apiProduct.stockQuantity > 0,
      freeShipping: apiProduct.freeShipping,
      hasGift: apiProduct.freeGift,
      isNew: apiProduct.isNew,
      description: apiProduct.description || ''
    };

    // Set up product images
    if (apiProduct.imageUrl) {
      this.productImages = [
        { id: 1, url: apiProduct.imageUrl, isMain: true }
      ];
      this.selectedImage = this.productImages[0];
    } else {
      this.productImages = [
        { id: 1, url: 'assets/images/prod1.png', isMain: true }
      ];
      this.selectedImage = this.productImages[0];
    }

    // Map variants to color/memory options
    if (apiProduct.variants && apiProduct.variants.length > 0) {
      this.mapVariants(apiProduct.variants);
    } else {
      // Default variants if none exist
      this.setupDefaultVariants();
    }

    // Set up product features (mock data for now)
    this.productFeatures = [
      'High quality construction',
      'Premium materials',
      'Extended warranty available'
    ];

    // Save to recently viewed
    this.saveToRecentlyViewed(apiProduct);
  }

  private saveToRecentlyViewed(product: any): void {
    const recentProduct = {
      id: product.id,
      name: product.name,
      price: product.price,
      image: product.imageUrl,
      rating: product.averageRating || 0,
      reviews: product.reviewCount || 0,
      isNew: product.isNew
    };

    // Get existing recently viewed products
    let recentlyViewed = [];
    const storedData = localStorage.getItem('recentlyViewedProducts');
    if (storedData) {
      recentlyViewed = JSON.parse(storedData);
    }

    // Remove if already exists and add to beginning
    recentlyViewed = recentlyViewed.filter((p: any) => p.id !== product.id);
    recentlyViewed.unshift(recentProduct);

    // Keep only last 10 items
    recentlyViewed = recentlyViewed.slice(0, 10);

    // Save back to localStorage
    localStorage.setItem('recentlyViewedProducts', JSON.stringify(recentlyViewed));
  }

  private mapVariants(variants: any[]): void {
    // Group variants by type
    const colorVariantsData = variants.filter(v => v.color);
    const sizeVariantsData = variants.filter(v => v.size);

    // Map color variants
    if (colorVariantsData.length > 0) {
      this.colorVariants = colorVariantsData.map((v, index) => ({
        id: v.id.toString(),
        type: 'color',
        name: v.color || 'Default',
        value: v.color || '#000000',
        price: this.product.price + (v.priceAdjustment || 0),
        image: v.imageUrl || this.product.currentImage,
        inStock: v.stockQuantity > 0
      }));
      this.selectedColor = this.colorVariants[0];
    }

    // Map size variants (using as memory for electronics)
    if (sizeVariantsData.length > 0) {
      this.memoryVariants = sizeVariantsData.map((v, index) => ({
        id: v.id.toString(),
        type: 'size',
        name: v.size || 'Standard',
        value: v.size || 'Standard',
        price: this.product.price + (v.priceAdjustment || 0),
        inStock: v.stockQuantity > 0
      }));
      this.selectedMemory = this.memoryVariants[0];
    }

    // If no variants, set up defaults
    if (this.colorVariants.length === 0) {
      this.setupDefaultColorVariants();
    }
    if (this.memoryVariants.length === 0) {
      this.setupDefaultMemoryVariants();
    }
  }

  private setupDefaultVariants(): void {
    this.setupDefaultColorVariants();
    this.setupDefaultMemoryVariants();
  }

  private setupDefaultColorVariants(): void {
    this.colorVariants = [
      { id: '1', type: 'color', name: 'Default', value: '#000000', price: this.product.price, inStock: true }
    ];
    this.selectedColor = this.colorVariants[0];
  }

  private setupDefaultMemoryVariants(): void {
    this.memoryVariants = [
      { id: '1', type: 'size', name: 'Standard', value: 'Standard', price: this.product.price, inStock: true }
    ];
    this.selectedMemory = this.memoryVariants[0];
  }

  private loadRelatedProducts(productId: number): void {
    this.productService.getRelatedProducts(productId, 5)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (products: any[]) => {
          this.relatedProducts = products.map(p => ({
            id: p.id,
            name: p.name,
            price: p.price,
            originalPrice: p.oldPrice,
            image: p.imageUrl,
            rating: p.averageRating || 0,
            reviews: p.reviewCount || 0,
            freeShipping: p.freeShipping,
            discount: p.oldPrice ? p.oldPrice - p.price : 0,
            isNew: p.isNew
          }));
        },
        error: (error) => {
          console.error('Error loading related products:', error);
        }
      });
  }

  private loadRecentlyViewed(): void {
    // Load from localStorage or service
    const recentlyViewed = localStorage.getItem('recentlyViewedProducts');
    if (recentlyViewed) {
      this.recentlyViewedProducts = JSON.parse(recentlyViewed).slice(0, 4);
    }
  }

  private loadFrequentlyBoughtProducts(): void {
    if (!this.product?.id) return;
    
    const apiUrl = `${environment.apiUrl}/FrequentlyBoughtTogether/product/${this.product.id}`;
    
    this.http.get<any>(apiUrl)
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (response) => {
          if (response && response.products) {
            // Skip the first product as it's the main product
            this.frequentlyBoughtProducts = response.products.slice(1).map((item: any) => ({
              id: item.id,
              name: item.name,
              price: item.price,
              oldPrice: item.oldPrice,
              imageUrl: item.imageUrl || `assets/images/prod${item.id}.png`,
              selected: item.selected !== false // Default to true if not specified
            }));
          }
        },
        error: (error) => {
          console.error('Error loading frequently bought products:', error);
          // Fallback to static data if API fails
          this.frequentlyBoughtProducts = [
            { 
              id: 2, 
              name: 'BOSO 2 Wireless On Ear Headphone', 
              price: 369.00, 
              imageUrl: 'assets/images/prod2.png', 
              selected: true 
            },
            { 
              id: 3, 
              name: 'Opplo Watch Series 8 GPS + Cellular Stainless Stell Case with Milanese Loop', 
              price: 129.00, 
              imageUrl: 'assets/images/prod3.png', 
              selected: true 
            }
          ];
        }
      });
  }

  private checkWishlistStatus(): void {
    this.isInWishlist = this.wishlistService.isInWishlist(this.product.id);
  }

  selectImage(image: ProductImage): void {
    this.selectedImage = image;
  }

  selectColor(variant: ProductVariant): void {
    this.selectedColor = variant;
    // Update main image if variant has an image
    if (variant.image) {
      this.selectedImage = { id: 999, url: variant.image, isMain: false };
    }
    this.updatePrice();
  }

  selectMemory(variant: ProductVariant): void {
    this.selectedMemory = variant;
    this.updatePrice();
  }

  updatePrice(): void {
    // Update product price based on selected variants
    if (this.selectedMemory) {
      this.product.price = this.selectedMemory.price;
    }
  }

  increaseQuantity(): void {
    this.quantity++;
  }

  decreaseQuantity(): void {
    if (this.quantity > 1) {
      this.quantity--;
    }
  }

  addToCart(): void {
    const cartItem: CartItem = {
      id: this.product.id,
      name: `${this.product.name}${this.selectedColor ? ' - ' + this.selectedColor.name : ''}${this.selectedMemory ? ' ' + this.selectedMemory.value : ''}`,
      price: this.product.price,
      quantity: this.quantity,
      image: this.selectedImage.url,
      inStock: this.product.inStock ?? true,
      shipping: this.product.freeShipping ? 'free shipping' : undefined,
      rating: this.product.rating,
      reviews: this.product.reviews
    };
    
    this.cartService.addToCart(cartItem);
    // TODO: Show success message
  }

  buyWithPayPal(): void {
    // TODO: Implement PayPal checkout
    console.log('Buy with PayPal');
  }

  addToWishlist(): void {
    // TODO: Implement wishlist functionality
    console.log('Added to wishlist');
  }

  addToCompare(): void {
    // TODO: Implement compare functionality
    console.log('Added to compare');
  }

  setActiveTab(tab: string): void {
    this.activeTab = tab;
  }

  toggleFrequentlyBought(product: typeof this.frequentlyBoughtProducts[0]): void {
    product.selected = !product.selected;
  }

  getTotalFrequentlyBoughtPrice(): number {
    let total = this.product.price;
    this.frequentlyBoughtProducts.forEach(product => {
      if (product.selected) {
        total += product.price;
      }
    });
    return total;
  }

  addAllToCart(): void {
    // Add main product
    this.addToCart();
    
    // Add selected frequently bought products
    this.frequentlyBoughtProducts.forEach(product => {
      if (product.selected) {
        const cartItem: CartItem = {
          id: product.id,
          name: product.name,
          price: product.price,
          quantity: 1,
          image: product.imageUrl,
          inStock: true,
          shipping: 'free shipping'
        };
        this.cartService.addToCart(cartItem);
      }
    });
  }

  getStarArray(rating: number): boolean[] {
    const stars = [];
    for (let i = 1; i <= 5; i++) {
      stars.push(i <= rating);
    }
    return stars;
  }

  toggleWishlist(): void {
    const wishlistItem = {
      id: this.product.id,
      name: this.product.name,
      price: this.product.price,
      oldPrice: undefined, // ProductDetail doesn't have originalPrice
      imageUrl: this.selectedImage?.url || this.product.currentImage || '',
      categoryName: this.product.category
    };
    
    this.isInWishlist = this.wishlistService.toggleWishlist(wishlistItem);
    console.log(`${this.product.name} ${this.isInWishlist ? 'added to' : 'removed from'} wishlist`);
  }

  onProductImageError(event: Event): void {
    const imgElement = event.target as HTMLImageElement;
    if (imgElement) {
      imgElement.src = 'assets/images/prod1.png';
    }
  }

  onFrequentlyBoughtImageError(event: Event): void {
    const imgElement = event.target as HTMLImageElement;
    if (imgElement) {
      imgElement.src = 'assets/images/placeholder.png';
    }
  }

  onFrequentlyBoughtSelectionChange(): void {
    // This method is called when checkbox selection changes
    // The total price will be automatically recalculated via getTotalFrequentlyBoughtPrice()
    console.log('Frequently bought selection changed');
  }

  navigateToProduct(productId: number): void {
    if (productId) {
      this.router.navigate(['/product', productId]);
    }
  }
}
