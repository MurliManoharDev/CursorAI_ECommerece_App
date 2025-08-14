import { Component, OnInit, inject } from '@angular/core';
import { ProductService, Category } from '../../services/product.service';

@Component({
  selector: 'app-category-menu',
  templateUrl: './category-menu.component.html',
  styleUrls: ['./category-menu.component.scss']
})
export class CategoryMenuComponent implements OnInit {
  private productService = inject(ProductService);

  categories: Category[] = [];
  isLoading = false;
  errorMessage = '';
  activeSubmenu: string | null = null;

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.isLoading = true;
    this.errorMessage = '';
    
    this.productService.getCategories().subscribe({
      next: (categories) => {
        this.categories = categories;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Failed to load categories:', error);
        this.errorMessage = 'Failed to load categories';
        this.isLoading = false;
      }
    });
  }

  toggleSubmenu(categoryId: number, event: Event): void {
    event.preventDefault();
    event.stopPropagation();
    
    const categoryIdStr = categoryId.toString();
    if (this.activeSubmenu === categoryIdStr) {
      this.activeSubmenu = null; // Close if already open
    } else {
      this.activeSubmenu = categoryIdStr; // Open the clicked submenu
    }
  }

  isSubmenuOpen(categoryId: number): boolean {
    return this.activeSubmenu === categoryId.toString();
  }

  closeAllSubmenus(): void {
    this.activeSubmenu = null;
  }

  // Get icon for category based on name
  getCategoryIcon(category: Category): string {
    const CATEGORY_ICON_MAP: { [key: string]: string } = {
      'laptops': 'fa-laptop',
      'pc-computers': 'fa-desktop',
      'pc & computers': 'fa-desktop',
      'computers': 'fa-desktop',
      'cell-phones': 'fa-mobile-alt',
      'cell phones': 'fa-mobile-alt',
      'mobile phones': 'fa-mobile-alt',
      'phones': 'fa-mobile-alt',
      'tablets': 'fa-tablet-alt',
      'gaming-vr': 'fa-gamepad',
      'gaming & vr': 'fa-gamepad',
      'gaming': 'fa-gamepad',
      'networking': 'fa-wifi',
      'cameras': 'fa-camera',
      'sounds': 'fa-headphones',
      'audio': 'fa-headphones',
      'headphones': 'fa-headphones',
      'office': 'fa-briefcase',
      'storage-usb': 'fa-hdd',
      'storage': 'fa-hdd',
      'usb': 'fa-usb',
      'accessories': 'fa-plug',
      'clearance': 'fa-gem',
      'sale': 'fa-tags',
      'default': 'fa-th'
    };

    if (category.iconClass) {
      return category.iconClass;
    }
    
    const nameLower = category.name.toLowerCase();
    const slugLower = category.slug.toLowerCase();
    
    return CATEGORY_ICON_MAP[slugLower] || 
           CATEGORY_ICON_MAP[nameLower] || 
           CATEGORY_ICON_MAP['default'];
  }
}
