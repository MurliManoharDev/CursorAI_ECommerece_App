import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { ReactiveFormsModule, FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HeaderComponent } from './components/header/header.component';
import { TopBarComponent } from './components/top-bar/top-bar.component';
import { MainMenuComponent } from './components/main-menu/main-menu.component';
import { FooterComponent } from './components/footer/footer.component';
import { HomeComponent } from './components/home/home.component';
import { SliderComponent } from './components/slider/slider.component';
import { FeaturedBrandsComponent } from './components/featured-brands/featured-brands.component';
import { TopCategoriesComponent } from './components/top-categories/top-categories.component';
import { DealsComponent } from './components/deals/deals.component';
import { ProductCardComponent } from './components/product-card/product-card.component';
import { ProductCarouselComponent } from './components/product-carousel/product-carousel.component';
import { BrandNewComponent } from './components/brand-new/brand-new.component';
import { ProductListComponent } from './components/product-list/product-list.component';
import { PromoBannersComponent } from './components/promo-banners/promo-banners.component';
import { RecentlyViewedComponent } from './components/recently-viewed/recently-viewed.component';
import { CategoryMenuComponent } from './components/category-menu/category-menu.component';
import { PromoBannerComponent } from './components/promo-banner/promo-banner.component';
import { SimpleTopCategoriesComponent } from './components/simple-top-categories/simple-top-categories.component';
import { LoginComponent } from './components/login/login.component';
import { RegisterComponent } from './components/register/register.component';
import { CartComponent } from './components/cart/cart.component';
import { ProfileComponent } from './components/profile/profile.component';
import { ContactComponent } from './components/contact/contact.component';
import { AboutComponent } from './components/about/about.component';
import { CheckoutComponent } from './components/checkout/checkout.component';
import { ProductDetailComponent } from './components/product-detail/product-detail.component';
import { ProductListingComponent } from './components/product-listing/product-listing.component';
import { WishlistComponent } from './components/wishlist/wishlist.component';
import { ForgotPasswordComponent } from './components/forgot-password/forgot-password.component';
import { ResetPasswordComponent } from './components/reset-password/reset-password.component';
import { ProductCarouselTabsComponent } from './components/product-carousel-tabs/product-carousel-tabs.component';
import { OrderSuccessComponent } from './components/order-success/order-success.component';
import { MyOrdersComponent } from './components/my-orders/my-orders.component';

@NgModule({
  declarations: [
    AppComponent,
    HeaderComponent,
    TopBarComponent,
    MainMenuComponent,
    FooterComponent,
    HomeComponent,
    SliderComponent,
    FeaturedBrandsComponent,
    TopCategoriesComponent,
    DealsComponent,
    ProductCardComponent,
    ProductCarouselComponent,
    BrandNewComponent,
    ProductListComponent,
    PromoBannersComponent,
    RecentlyViewedComponent,
    CategoryMenuComponent,
    PromoBannerComponent,
    SimpleTopCategoriesComponent,
    LoginComponent,
    RegisterComponent,
    CartComponent,
    ProfileComponent,
    ContactComponent,
    AboutComponent,
    CheckoutComponent,
    ProductDetailComponent,
    ProductListingComponent,
    WishlistComponent,
    ForgotPasswordComponent,
    ResetPasswordComponent,
    ProductCarouselTabsComponent,
    OrderSuccessComponent,
    MyOrdersComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    HttpClientModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
