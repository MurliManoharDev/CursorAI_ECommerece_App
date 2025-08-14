import { Component, Input } from '@angular/core';
import { Router } from '@angular/router';
import { Product } from '../../models/product';

interface Tab {
  id: string;
  label: string;
  products: Product[];
  isLoading: boolean;
}

@Component({
  selector: 'app-product-carousel-tabs',
  templateUrl: './product-carousel-tabs.component.html',
  styleUrls: ['./product-carousel-tabs.component.scss']
})
export class ProductCarouselTabsComponent {
  @Input() bestSellers: Product[] = [];
  @Input() newProducts: Product[] = [];
  @Input() popularProducts: Product[] = [];
  @Input() isLoadingBestSellers: boolean = false;
  @Input() isLoadingNewProducts: boolean = false;
  @Input() isLoadingPopularProducts: boolean = false;

  activeTab: string = 'best-seller';

  constructor(private router: Router) {}
  
  get tabs(): Tab[] {
    return [
      {
        id: 'best-seller',
        label: 'BEST SELLER',
        products: this.bestSellers,
        isLoading: this.isLoadingBestSellers
      },
      {
        id: 'new-in',
        label: 'NEW IN',
        products: this.newProducts,
        isLoading: this.isLoadingNewProducts
      },
      {
        id: 'popular',
        label: 'POPULAR',
        products: this.popularProducts,
        isLoading: this.isLoadingPopularProducts
      }
    ];
  }

  get activeTabData(): Tab | undefined {
    return this.tabs.find(tab => tab.id === this.activeTab);
  }

  setActiveTab(tabId: string): void {
    this.activeTab = tabId;
  }

  onViewAll(): void {
    // Navigate to products page with filter based on active tab
    const queryParams: any = {};
    
    switch (this.activeTab) {
      case 'best-seller':
        queryParams.sort = 'sales';
        break;
      case 'new-in':
        queryParams.filter = 'new';
        break;
      case 'popular':
        queryParams.sort = 'popularity';
        break;
    }
    
    this.router.navigate(['/products'], { queryParams });
  }
}
