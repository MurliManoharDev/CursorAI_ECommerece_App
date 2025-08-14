import { Component, OnInit } from '@angular/core';
import { Product, Category, PromotionalItem } from '../../models/product';
import { ProductService, Product as ApiProduct } from '../../services/product.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss']
})
export class HomeComponent implements OnInit {

  bestSellers: Product[] = [];
  newProducts: Product[] = [];
  popularProducts: Product[] = [];
  cellphones: Product[] = [];
  laptops: Product[] = [];

  // Loading states
  isLoadingBestSellers = true;
  isLoadingNewProducts = true;
  isLoadingPopularProducts = true;

  // Data for Figma design
  cellphoneCategories: Category[] = [];
  cellphonePromo: PromotionalItem = {} as PromotionalItem;
  laptopCategories: Category[] = [];
  laptopPromo: PromotionalItem = {} as PromotionalItem;

  constructor(private productService: ProductService) {}

  ngOnInit(): void {
    this.loadProductCarousels();
    this.loadCategoryProducts();
    this.loadStaticData();
  }

  private loadProductCarousels(): void {
    // Load best seller products
    this.productService.getBestSellerProducts(10).subscribe({
      next: (products) => {
        this.bestSellers = this.mapApiProductsToModel(products);
        this.isLoadingBestSellers = false;
      },
      error: (error) => {
        console.error('Error loading best seller products:', error);
        this.isLoadingBestSellers = false;
        // Fallback to static data
        this.loadStaticBestSellers();
      }
    });

    // Load new products
    this.productService.getNewProducts(10).subscribe({
      next: (products) => {
        this.newProducts = this.mapApiProductsToModel(products);
        this.isLoadingNewProducts = false;
      },
      error: (error) => {
        console.error('Error loading new products:', error);
        this.isLoadingNewProducts = false;
        // Fallback to static data
        this.loadStaticNewProducts();
      }
    });

    // Load popular products
    this.productService.getPopularProducts(10).subscribe({
      next: (products) => {
        this.popularProducts = this.mapApiProductsToModel(products);
        this.isLoadingPopularProducts = false;
      },
      error: (error) => {
        console.error('Error loading popular products:', error);
        this.isLoadingPopularProducts = false;
        // Fallback to static data
        this.loadStaticPopularProducts();
      }
    });
  }

  private loadCategoryProducts(): void {
    // Load products for cellphones category (assuming category ID 1 for phones)
    this.productService.getProducts({ pageNumber: 1, pageSize: 8, categoryId: 1 }).subscribe({
      next: (result) => {
        this.cellphones = this.mapApiProductsToModel(result.items);
      },
      error: (error) => {
        console.error('Error loading cellphone products:', error);
        this.loadStaticCellphones();
      }
    });

    // Load products for laptops category (assuming category ID 2 for laptops)
    this.productService.getProducts({ pageNumber: 1, pageSize: 5, categoryId: 2 }).subscribe({
      next: (result) => {
        this.laptops = this.mapApiProductsToModel(result.items);
      },
      error: (error) => {
        console.error('Error loading laptop products:', error);
        this.loadStaticLaptops();
      }
    });
  }

  private mapApiProductsToModel(apiProducts: ApiProduct[]): Product[] {
    return apiProducts.map(apiProduct => ({
      id: apiProduct.id,
      name: apiProduct.name,
      subtitle: apiProduct.subtitle,
      imageUrl: apiProduct.imageUrl,
      price: apiProduct.price,
      oldPrice: apiProduct.oldPrice,
      rating: apiProduct.averageRating || 0,
      reviews: apiProduct.reviewCount || 0,
      inStock: true, // We don't have stock info from the API, assuming in stock
      tags: this.generateTags(apiProduct),
      new: apiProduct.isOnSale ? false : true, // Assuming non-sale items are new
      freeShipping: apiProduct.freeShipping
    }));
  }

  private generateTags(product: ApiProduct): string[] {
    const tags: string[] = [];
    if (product.freeShipping) {
      tags.push('free shipping');
    }
    if (product.isOnSale && product.oldPrice) {
      const discount = Math.round(((product.oldPrice - product.price) / product.oldPrice) * 100);
      tags.push(`${discount}% OFF`);
    }
    return tags;
  }

  private loadStaticData(): void {
    // Static categories data
    this.cellphoneCategories = [
      { id: 1, name: 'iPhone (iOS)', imageUrl: 'assets/images/prod20.png', itemCount: 74 },
      { id: 2, name: 'Android', imageUrl: 'assets/images/prod21.png', itemCount: 35 },
      { id: 3, name: '5G Support', imageUrl: 'assets/images/prod22.png', itemCount: 12 },
      { id: 4, name: 'Gaming', imageUrl: 'assets/images/prod23.png', itemCount: 9 },
      { id: 5, name: 'Xiaomi', imageUrl: 'assets/images/prod24.png', itemCount: 52 },
      { id: 6, name: 'Accessories', imageUrl: 'assets/images/prod25.png', itemCount: 29 }
    ];

    this.cellphonePromo = {
      id: 1,
      title: 'Top 100',
      subtitle: 'Cellphones & Accessories',
      description: 'Guaranteed Best Brands. Period.',
      imageUrl: 'assets/images/phones-promo.png',
      buttonText: 'SHOP NOW',
      buttonLink: '/cellphones'
    };

    this.laptopCategories = [
      { id: 1, name: 'Gaming Laptops', imageUrl: 'assets/images/prod37.png', itemCount: 45 },
      { id: 2, name: 'Business', imageUrl: 'assets/images/prod38.png', itemCount: 28 },
      { id: 3, name: 'Ultrabooks', imageUrl: 'assets/images/prod39.png', itemCount: 33 },
      { id: 4, name: 'Workstations', imageUrl: 'assets/images/prod40.png', itemCount: 16 },
      { id: 5, name: 'Budget', imageUrl: 'assets/images/prod41.png', itemCount: 67 },
      { id: 6, name: 'Accessories', imageUrl: 'assets/images/prod42.jpg', itemCount: 89 }
    ];

    this.laptopPromo = {
      id: 2,
      title: 'Professional',
      subtitle: 'Laptops & Computers',
      description: 'Unleash Creativity Anywhere',
      imageUrl: 'assets/images/laptops-promo.png',
      buttonText: 'EXPLORE',
      buttonLink: '/laptops'
    };
  }

  // Fallback static data methods
  private loadStaticBestSellers(): void {
    this.bestSellers = [
      { id: 999001, name: 'BOSO 2 Wireless On Ear Headphone', imageUrl: 'assets/images/prod9.png', price: 359, rating: 5, reviews: 152, inStock: true, tags: ['free shipping', 'free gift'] },
      { id: 999002, name: 'OPod Pro 12.9 Inch M1 2023, 64GB + Wifi, GPS', imageUrl: 'assets/images/prod10.png', price: 569, oldPrice: 759, rating: 5, reviews: 152, inStock: true, tags: ['free shipping'] },
      { id: 999003, name: 'uLosk Mini case 2.0, Xenon i10 / 32GB / SSD 512GB / VGA 8GB', imageUrl: 'assets/images/prod11.png', price: 1729, oldPrice: 2119, rating: 4, reviews: 8, inStock: false },
      { name: 'Opplo Watch Series 8 GPS + Cellular Stainless Steel Case with Milanese Loop', imageUrl: 'assets/images/prod12.png', price: 979, rating: 0, reviews: 0, inStock: true, tags: ['$2.98 Shipping'] },
      { name: 'iSmart 24V Charger', imageUrl: 'assets/images/prod13.png', price: 9, oldPrice: 12, rating: 5, reviews: 9, inStock: false, tags: ['$3.98 Shipping'] }
    ];
  }

  private loadStaticNewProducts(): void {
    this.newProducts = [
      { name: 'iSmart 24V Charger', imageUrl: 'assets/images/prod13.png', price: 9, oldPrice: 12, rating: 5, reviews: 9, inStock: false, tags: ['$3.98 Shipping'] },
      { name: 'Opplo Watch Series 8 GPS + Cellular Stainless Steel Case with Milanese Loop', imageUrl: 'assets/images/prod12.png', price: 979, rating: 0, reviews: 0, inStock: true, tags: ['$2.98 Shipping'] },
      { name: 'uLosk Mini case 2.0, Xenon i10 / 32GB / SSD 512GB / VGA 8GB', imageUrl: 'assets/images/prod11.png', price: 1729, oldPrice: 2119, rating: 4, reviews: 8, inStock: false },
      { name: 'OPod Pro 12.9 Inch M1 2023, 64GB + Wifi, GPS', imageUrl: 'assets/images/prod10.png', price: 569, oldPrice: 759, rating: 5, reviews: 152, inStock: true, tags: ['free shipping'] },
      { name: 'BOSO 2 Wireless On Ear Headphone', imageUrl: 'assets/images/prod9.png', price: 359, rating: 5, reviews: 152, inStock: true, tags: ['free shipping', 'free gift'] }
    ];
  }

  private loadStaticPopularProducts(): void {
    this.popularProducts = [...this.bestSellers];
  }

  private loadStaticCellphones(): void {
    this.cellphones = [
      { id: 1, name: 'SROK Smart Phone 128GB, Oled Retina', imageUrl: 'assets/images/prod26.png', price: 579, oldPrice: 859, rating: 5, reviews: 152, inStock: true, freeShipping: true, savings: 199 },
      { id: 2, name: 'aPod Pro Tablet 2023 LTE + Wifi, GPS Cellular 12.9 Inch, 512GB', imageUrl: 'assets/images/prod27.png', price: 979, rating: 0, reviews: 0, inStock: true, new: true, shippingCost: 2.98 },
      { id: 3, name: 'OPod Pro 12.9 Inch M1 2023, 64GB + Wifi, GPS', imageUrl: 'assets/images/prod28.png', price: 659, rating: 5, reviews: 5, inStock: true, freeShipping: true, freeGift: true, variants: [
        { imageUrl: 'assets/images/prod28.png', color: 'Space Grey' },
        { imageUrl: 'assets/images/prod28-pink.png', color: 'Pink' },
        { imageUrl: 'assets/images/prod28-blue.png', color: 'Blue' }
      ] },
      { id: 4, name: 'Xiamoi Redmi Note 5, 64GB', imageUrl: 'assets/images/prod29.png', price: 1239, oldPrice: 1619, rating: 5, reviews: 9, inStock: false, contactForPrice: true, savings: 59 },
      { id: 5, name: 'Microsute Alpha Ultra S5 Surface 128GB 2022, Sliver', imageUrl: 'assets/images/prod30.png', price: 1729, rating: 4, reviews: 8, inStock: false, contactForPrice: true, variants: [
        { imageUrl: 'assets/images/prod30.png', color: 'Silver' },
        { imageUrl: 'assets/images/prod30-black.png', color: 'Black' }
      ] },
      { id: 6, name: 'Xperia Pro Smartphone 256GB, Camera Edition', imageUrl: 'assets/images/prod31.png', price: 899, oldPrice: 1199, rating: 4, reviews: 12, inStock: true, freeShipping: true, savings: 300, new: true },
      { id: 7, name: 'Galaxy S23 Ultra 5G 128GB', imageUrl: 'assets/images/prod32.png', price: 1099, oldPrice: 1299, rating: 5, reviews: 87, inStock: true, freeShipping: true, savings: 200 },
      { id: 8, name: 'iPhone 14 Pro Max 256GB', imageUrl: 'assets/images/prod33.png', price: 1299, rating: 5, reviews: 203, inStock: true, freeShipping: true, new: true }
    ];
  }

  private loadStaticLaptops(): void {
    this.laptops = [
      { name: 'Pineapple Macbook Pro 2022 M1 / 512 GB', imageUrl: 'assets/images/prod37.png', price: 579, rating: 5, reviews: 152, inStock: true, tags: ['free shipping'], new: true },
      { name: 'C&O Bluetooth Speaker', imageUrl: 'assets/images/prod38.png', price: 979, rating: 0, reviews: 0, inStock: true, new: true },
      { name: 'Gigaby Custome Case, i7/ 16GB / SSD 256GB', imageUrl: 'assets/images/prod39.png', price: 1259, rating: 5, reviews: 5, inStock: true, tags: ['free shipping', 'free gift'] },
      { name: 'BEOS PC Gaming Case', imageUrl: 'assets/images/prod40.png', price: 1239, oldPrice: 1619, rating: 5, reviews: 9, inStock: false },
      { name: 'aMoc All-in-one Computer M1', imageUrl: 'assets/images/prod41.png', price: 1729, rating: 4, reviews: 8, inStock: false }
    ];
  }

}
