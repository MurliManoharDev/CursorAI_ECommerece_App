import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';

interface TopCategory {
  id: string;
  title: string;
  viewAllLink?: string;
  categoryImage?: string;
  featuredProduct: {
    id?: number;
    title: string;
    description?: string;
    imageUrl?: string;
    backgroundColor?: string;
    image?: string;
  };
  subcategories: {
    id?: string;
    name: string;
    itemCount?: number;
    imageUrl?: string;
    backgroundColor?: string;
  }[];
}

@Component({
  selector: 'app-top-categories',
  templateUrl: './top-categories.component.html',
  styleUrls: ['./top-categories.component.scss']
})
export class TopCategoriesComponent implements OnInit {
  mainCategories: TopCategory[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(
    private productService: ProductService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadFeaturedCategories();
  }

  loadFeaturedCategories(): void {
    this.isLoading = true;
    this.productService.getFeaturedCategories().subscribe({
      next: (categories) => {
        this.mainCategories = categories.map(cat => ({
          id: cat.slug,
          title: cat.name.toUpperCase(),
          viewAllLink: `/products/category/${cat.slug}`,
          featuredProduct: {
            id: cat.featuredProduct?.id,
            title: cat.featuredProduct?.name || 'Featured Product',
            description: cat.featuredProduct?.description,
            imageUrl: cat.featuredProduct?.imageUrl || 'assets/images/placeholder.png',
            backgroundColor: cat.featuredProduct?.backgroundColor || '#334155'
          },
          subcategories: cat.subcategories.map((sub: any) => ({
            id: sub.slug,
            name: sub.name,
            itemCount: sub.productCount,
            imageUrl: sub.imageUrl || 'assets/images/placeholder.png',
            backgroundColor: sub.backgroundColor || '#E2E8F0'
          }))
        }));
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading featured categories:', error);
        this.errorMessage = 'Failed to load categories. Please try again later.';
        this.isLoading = false;
        // Fallback to static data if API fails
        this.loadStaticData();
      }
    });
  }

  onViewAll(categoryId: string): void {
    this.router.navigate(['/products/category', categoryId]).then(() => {
      window.scrollTo(0, 0);
    });
  }

  onCategoryClick(category: TopCategory, subcategory?: TopCategory['subcategories'][0]): void {
    if (subcategory) {
      // Navigate to subcategory
      this.router.navigate(['/products/category', category.id, 'subcategory', subcategory.id]).then(() => {
        window.scrollTo(0, 0);
      });
    } else {
      // Navigate to featured product
      if (category.featuredProduct.id) {
        this.router.navigate(['/product', category.featuredProduct.id]).then(() => {
          window.scrollTo(0, 0);
        });
      } else {
        // If no product ID, go to category page
        this.router.navigate(['/products/category', category.id]).then(() => {
          window.scrollTo(0, 0);
        });
      }
    }
  }

  private loadStaticData(): void {
    // ... existing static data for fallback
    this.mainCategories = [
      {
        id: 'audios-cameras',
        title: 'AUDIOS & CAMERAS',
        viewAllLink: '#',
        featuredProduct: {
          title: 'Best Speaker 2023',
          description: 'Premium sound quality',
          imageUrl: 'assets/images/prod1.png',
          backgroundColor: '#334155'
        },
        subcategories: [
          { 
            name: 'Speaker', 
            itemCount: 12, 
            imageUrl: 'assets/images/prod2.png',
            backgroundColor: '#E2E8F0'
          },
          { 
            name: 'DSLR Camera', 
            itemCount: 9, 
            imageUrl: 'assets/images/prod3.png',
            backgroundColor: '#E2E8F0'
          },
          { 
            name: 'Earbuds', 
            itemCount: 5, 
            imageUrl: 'assets/images/prod4.png',
            backgroundColor: '#E2E8F0'
          },
          { 
            name: 'Microphone', 
            itemCount: 12, 
            imageUrl: 'assets/images/prod5.png',
            backgroundColor: '#E2E8F0'
          }
        ]
      },
      {
        id: 'gaming',
        title: 'GAMING',
        viewAllLink: '#',
        featuredProduct: {
          title: 'WIRELESS RGB GAMING MOUSE',
          description: 'High precision gaming',
          imageUrl: 'assets/images/prod6.png',
          backgroundColor: '#F1F5F9'
        },
        subcategories: [
          { 
            name: 'Monitors', 
            itemCount: 28, 
            imageUrl: 'assets/images/prod7.png',
            backgroundColor: '#E2E8F0'
          },
          { 
            name: 'Chair', 
            itemCount: 12, 
            imageUrl: 'assets/images/prod8.png',
            backgroundColor: '#E2E8F0'
          },
          { 
            name: 'Controller', 
            itemCount: 9, 
            imageUrl: 'assets/images/prod9.png',
            backgroundColor: '#E2E8F0'
          },
          { 
            name: 'Keyboards', 
            itemCount: 30, 
            imageUrl: 'assets/images/prod10.png',
            backgroundColor: '#E2E8F0'
          }
        ]
      },
      {
        id: 'office-equipments',
        title: 'OFFICE EQUIPMENTS',
        viewAllLink: '#',
        featuredProduct: {
          title: 'Laser Projector',
          description: 'Home Thearther 4K',
          imageUrl: 'assets/images/prod11.png',
          backgroundColor: '#374151'
        },
        subcategories: [
          { 
            name: 'Printers', 
            itemCount: 9, 
            imageUrl: 'assets/images/prod12.png',
            backgroundColor: '#E2E8F0'
          },
          { 
            name: 'Network', 
            itemCount: 90, 
            imageUrl: 'assets/images/prod13.png',
            backgroundColor: '#E2E8F0'
          },
          { 
            name: 'Security', 
            itemCount: 12, 
            imageUrl: 'assets/images/prod14.jpg',
            backgroundColor: '#E2E8F0'
          },
          { 
            name: 'Projectors', 
            itemCount: 12, 
            imageUrl: 'assets/images/prod15.jpg',
            backgroundColor: '#E2E8F0'
          }
        ]
      }
    ];
  }

} 