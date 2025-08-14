import { Component, OnInit } from '@angular/core';
import { Product } from '../../models/product';

@Component({
  selector: 'app-recently-viewed',
  templateUrl: './recently-viewed.component.html',
  styleUrls: ['./recently-viewed.component.scss']
})
export class RecentlyViewedComponent implements OnInit {

  recentlyViewed: Product[] = [];

  ngOnInit(): void {
    this.recentlyViewed = [
      { name: 'Xomie Remid 8 Sport Water Resitance Watch', imageUrl: 'assets/images/prod58.png', price: 579, rating: 5, reviews: 152, inStock: true, new: true },
      { name: 'Microte Surface 2.0 Laptop', imageUrl: 'assets/images/prod59.png', price: 979, rating: 0, reviews: 0, inStock: true, new: true },
      { name: 'aPod Pro Tablet 2023 LTE + Wifi, GPS Cellular 12.9 Inch, 512GB', imageUrl: 'assets/images/prod60.png', price: 979, rating: 0, reviews: 0, inStock: true },
      { name: 'SROK Smart Phone 128GB, Oled Retina', imageUrl: 'assets/images/prod61.png', price: 579, oldPrice: 779, rating: 5, reviews: 152, inStock: true }
    ];
  }

}
