import { Component, Input, OnInit, HostListener } from '@angular/core';
import { Product, Category, PromotionalItem } from '../../models/product';

@Component({
  selector: 'app-product-list',
  templateUrl: './product-list.component.html',
  styleUrls: ['./product-list.component.scss']
})
export class ProductListComponent implements OnInit {

  @Input() title!: string;
  @Input() products!: Product[];
  @Input() categories!: Category[];
  @Input() promotionalItem!: PromotionalItem;
  @Input() showViewAll = true;

  currentSlide = 0;
  maxSlides = 6; // Default number of products to show at once

  ngOnInit(): void {
    this.updateMaxSlides();
  }

  @HostListener('window:resize')
  onResize() {
    this.updateMaxSlides();
    // Reset currentSlide if needed
    if (this.currentSlide >= this.products.length - this.maxSlides) {
      this.currentSlide = Math.max(0, this.products.length - this.maxSlides);
    }
  }

  updateMaxSlides(): void {
    const width = window.innerWidth;
    if (width <= 480) {
      this.maxSlides = 1;
    } else if (width <= 768) {
      this.maxSlides = 2;
    } else if (width <= 992) {
      this.maxSlides = 3;
    } else if (width <= 1200) {
      this.maxSlides = 4;
    } else if (width <= 1400) {
      this.maxSlides = 5;
    } else {
      this.maxSlides = 6;
    }
  }

  nextSlide(): void {
    if (this.currentSlide < this.products.length - this.maxSlides) {
      this.currentSlide++;
    }
  }

  previousSlide(): void {
    if (this.currentSlide > 0) {
      this.currentSlide--;
    }
  }

  getStarArray(rating: number): number[] {
    return Array(5).fill(0).map((_, i) => i < rating ? 1 : 0);
  }

  getVisibleProducts(): Product[] {
    if (this.products.length <= this.maxSlides) {
      return this.products;
    }
    return this.products.slice(this.currentSlide, this.currentSlide + this.maxSlides);
  }

  showNavigation(): boolean {
    return this.products.length > this.maxSlides && this.maxSlides > 1;
  }
}
