import { Component } from '@angular/core';

@Component({
  selector: 'app-brand-new',
  templateUrl: './brand-new.component.html',
  styleUrls: ['./brand-new.component.scss']
})
export class BrandNewComponent {

  brandItems = [
    {
      title: 'Zumac Steel Computer Case',
      subtitle: 'And an option to upgrade every three years',
      imageUrl: 'assets/images/prod17.jpg'
    },
    {
      title: 'Summer Sale with Sale up to 50% OFF for Foam Gaming Chair.',
      subtitle: 'Limited time offer. Hurry up',
      imageUrl: 'assets/images/prod16.jpg'
    },
    {
      title: 'Summer Sale with Sale up to 50% OFF for Foam Gaming Chair.',
      subtitle: 'Limited time offer. Hurry up',
      imageUrl: 'assets/images/prod14.jpg'
    },
    {
      title: 'iPed Pro Mini 6 - Powerful l in hand',
      subtitle: 'From $19.99/month for 36 months. $280.35 final payment due in month 37',
      imageUrl: 'assets/images/prod15.jpg'
    }
  ];

  currentSlide = 0;
  itemsPerView = 4; // Number of items to show at once
  
  get maxSlides(): number {
    return Math.max(0, this.brandItems.length - this.itemsPerView);
  }

  getVisibleItems(): typeof this.brandItems {
    const start = this.currentSlide;
    const end = start + this.itemsPerView;
    return this.brandItems.slice(start, end);
  }

  previousSlide(): void {
    if (this.currentSlide > 0) {
      this.currentSlide--;
    }
  }

  nextSlide(): void {
    if (this.currentSlide < this.maxSlides) {
      this.currentSlide++;
    }
  }

  onShopNow(item: typeof this.brandItems[0]): void {
    console.log('Shop now clicked for:', item.title);
    // Add your navigation logic here
    // For example: this.router.navigate(['/product', item.id]);
  }

}
