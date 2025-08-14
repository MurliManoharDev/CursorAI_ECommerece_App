import { Component, Input } from '@angular/core';
import { Product, Category, PromotionalItem } from '../../models/product';

@Component({
  selector: 'app-deals',
  templateUrl: './deals.component.html',
  styleUrls: ['./deals.component.scss']
})
export class DealsComponent {
  @Input() products: Product[] = [];
  @Input() categories: Category[] = [];
  @Input() promo: PromotionalItem = {} as PromotionalItem;
}
