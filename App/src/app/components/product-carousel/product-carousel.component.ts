import { Component, Input, ViewChild, ElementRef, AfterViewInit, HostListener, OnChanges, SimpleChanges } from '@angular/core';
import { Product } from '../../models/product';

@Component({
  selector: 'app-product-carousel',
  templateUrl: './product-carousel.component.html',
  styleUrls: ['./product-carousel.component.scss']
})
export class ProductCarouselComponent implements AfterViewInit, OnChanges {

  @Input() title!: string;
  @Input() products: Product[] = [];
  @Input() isLoading: boolean = false;

  @ViewChild('carouselContainer') carouselContainer!: ElementRef<HTMLDivElement>;
  @ViewChild('carouselTrack') carouselTrack!: ElementRef<HTMLDivElement>;

  canScrollPrev = false;
  canScrollNext = true;
  scrollAmount = 0;

  ngAfterViewInit(): void {
    setTimeout(() => {
      this.checkScrollButtons();
    }, 100);
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['products'] && !changes['products'].firstChange) {
      setTimeout(() => {
        this.checkScrollButtons();
      }, 100);
    }
  }

  @HostListener('window:resize')
  onResize(): void {
    this.checkScrollButtons();
  }

  scrollPrev(): void {
    if (this.carouselContainer) {
      const container = this.carouselContainer.nativeElement;
      const scrollAmount = container.offsetWidth * 0.8; // Scroll 80% of container width
      container.scrollBy({
        left: -scrollAmount,
        behavior: 'smooth'
      });
      setTimeout(() => this.checkScrollButtons(), 300);
    }
  }

  scrollNext(): void {
    if (this.carouselContainer) {
      const container = this.carouselContainer.nativeElement;
      const scrollAmount = container.offsetWidth * 0.8; // Scroll 80% of container width
      container.scrollBy({
        left: scrollAmount,
        behavior: 'smooth'
      });
      setTimeout(() => this.checkScrollButtons(), 300);
    }
  }

  private checkScrollButtons(): void {
    if (this.carouselContainer && this.carouselContainer.nativeElement) {
      const container = this.carouselContainer.nativeElement;
      this.canScrollPrev = container.scrollLeft > 0;
      this.canScrollNext = container.scrollLeft < 
        (container.scrollWidth - container.offsetWidth - 5); // 5px threshold
    }
  }

  onScroll(container: HTMLDivElement): void {
    this.checkScrollButtons();
  }
}
