import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-promo-banner',
  templateUrl: './promo-banner.component.html',
  styleUrls: ['./promo-banner.component.scss']
})
export class PromoBannerComponent {
  @Input() imageSrc = 'assets/images/ban1.png';
  @Input() altText = 'Promotional Banner';
} 