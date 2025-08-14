import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProductService } from '../../services/product.service';

interface SimpleCategory {
  id: string;
  name: string;
  imageUrl: string;
}

@Component({
  selector: 'app-simple-top-categories',
  templateUrl: './simple-top-categories.component.html',
  styleUrls: ['./simple-top-categories.component.scss']
})
export class SimpleTopCategoriesComponent implements OnInit {
  categories: SimpleCategory[] = [];
  isLoading = true;
  errorMessage = '';

  constructor(
    private productService: ProductService,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.loadTopCategories();
  }

  loadTopCategories(): void {
    this.isLoading = true;
    this.productService.getCategories().subscribe({
      next: (categories) => {
        // Take first 4 categories and map to simple format
        this.categories = categories
          .slice(0, 4)
          .map(cat => ({
            id: cat.slug,
            name: cat.name,
            imageUrl: cat.imageUrl || this.getCategoryDefaultImage(cat.name)
          }));
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading categories:', error);
        this.errorMessage = 'Failed to load categories';
        this.isLoading = false;
        // Fallback to static data
        this.loadStaticCategories();
      }
    });
  }

  onCategoryClick(category: SimpleCategory): void {
    this.router.navigate(['/products/category', category.id]).then(() => {
      window.scrollTo(0, 0);
    });
  }

  onViewAll(): void {
    this.router.navigate(['/products']).then(() => {
      window.scrollTo(0, 0);
    });
  }

  onImageError(event: Event): void {
    const imgElement = event.target as HTMLImageElement;
    imgElement.src = 'assets/images/prod1.png';  // Using existing product image as placeholder
  }

  private getCategoryDefaultImage(categoryName: string): string {
    // Return appropriate default image based on category name
    const name = categoryName.toLowerCase();
    if (name.includes('laptop') || name.includes('computer')) {
      return 'assets/images/prod1.png';  // Laptop image
    } else if (name.includes('gaming') || name.includes('pc')) {
      return 'assets/images/prod2.png';  // Gaming-related image
    } else if (name.includes('headphone') || name.includes('audio')) {
      return 'assets/images/prod3.png';  // Audio-related image
    } else if (name.includes('monitor') || name.includes('display')) {
      return 'assets/images/prod4.png';  // Monitor-related image
    }
    return 'assets/images/prod1.png';  // Default fallback
  }

  private loadStaticCategories(): void {
    // Static fallback data
    this.categories = [
      {
        id: 'laptops',
        name: 'Laptops',
        imageUrl: 'assets/images/prod1.png'
      },
      {
        id: 'pc-gaming',
        name: 'PC Gaming',
        imageUrl: 'assets/images/prod2.png'
      },
      {
        id: 'headphones',
        name: 'Headphones',
        imageUrl: 'assets/images/prod3.png'
      },
      {
        id: 'monitors',
        name: 'Monitors',
        imageUrl: 'assets/images/prod4.png'
      }
    ];
  }
}
