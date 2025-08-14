import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BrandService, Brand } from '../../services/brand.service';

@Component({
  selector: 'app-featured-brands',
  templateUrl: './featured-brands.component.html',
  styleUrls: ['./featured-brands.component.scss']
})
export class FeaturedBrandsComponent implements OnInit {
  brands: Brand[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(
    private brandService: BrandService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadFeaturedBrands();
  }

  loadFeaturedBrands(): void {
    this.isLoading = true;
    this.brandService.getFeaturedBrands().subscribe({
      next: (brands) => {
        this.brands = brands.slice(0, 10); // Take only first 10 brands
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading featured brands:', error);
        this.errorMessage = 'Failed to load brands';
        this.isLoading = false;
        // Fallback to static brands if API fails
        this.loadStaticBrands();
      }
    });
  }

  onBrandClick(brand: Brand): void {
    // Navigate to products filtered by brand
    this.router.navigate(['/products'], { queryParams: { brand: brand.id } });
  }

  onViewAll(): void {
    // Navigate to all brands page
    this.router.navigate(['/brands']);
  }

  onImageError(event: Event): void {
    const imgElement = event.target as HTMLImageElement;
    imgElement.src = 'assets/images/logo.png';  // Using existing logo.png as placeholder
  }

  private loadStaticBrands(): void {
    // Static fallback data
    this.brands = [
      { id: 1, name: 'Brand 1', logoUrl: 'assets/images/logo1.png', isFeatured: true, displayOrder: 1, productCount: 0 },
      { id: 2, name: 'Brand 2', logoUrl: 'assets/images/logo2.png', isFeatured: true, displayOrder: 2, productCount: 0 },
      { id: 3, name: 'Brand 3', logoUrl: 'assets/images/logo3.png', isFeatured: true, displayOrder: 3, productCount: 0 },
      { id: 4, name: 'Brand 4', logoUrl: 'assets/images/logo4.png', isFeatured: true, displayOrder: 4, productCount: 0 },
      { id: 5, name: 'Brand 5', logoUrl: 'assets/images/logo5.png', isFeatured: true, displayOrder: 5, productCount: 0 },
      { id: 6, name: 'Brand 6', logoUrl: 'assets/images/logo6.png', isFeatured: true, displayOrder: 6, productCount: 0 },
      { id: 7, name: 'Brand 7', logoUrl: 'assets/images/logo7.png', isFeatured: true, displayOrder: 7, productCount: 0 },
      { id: 8, name: 'Brand 8', logoUrl: 'assets/images/logo8.png', isFeatured: true, displayOrder: 8, productCount: 0 },
      { id: 9, name: 'Brand 9', logoUrl: 'assets/images/logo9.png', isFeatured: true, displayOrder: 9, productCount: 0 },
      { id: 10, name: 'Brand 10', logoUrl: 'assets/images/logo10.png', isFeatured: true, displayOrder: 10, productCount: 0 }
    ];
  }
}
