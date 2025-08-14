# Placeholder Images

This application uses existing images as placeholders when brand or category images are not available.

## Current Placeholder Images:

### For Featured Brands Section:
- **Default placeholder**: `logo.png` - Generic logo used when brand logo is missing
- **Fallback images**: `logo1.png` through `logo10.png` are used for static brand data

### For Simple Top Categories Section:
- **Laptop/Computer categories**: `prod1.png`
- **Gaming/PC categories**: `prod2.png`
- **Headphone/Audio categories**: `prod3.png`
- **Monitor/Display categories**: `prod4.png`
- **Default fallback**: `prod1.png`

## Adding Custom Placeholder Images:

If you want to use custom placeholder images instead of the existing ones:

1. Create these files:
   - `placeholder-brand.png` - Generic brand/logo placeholder
   - `category-default.png` - Generic category placeholder

2. Update the component files to use these new placeholders:
   - `featured-brands.component.ts` - Update `onImageError()` method
   - `simple-top-categories.component.ts` - Update `onImageError()` and `getCategoryDefaultImage()` methods

## Recommended dimensions:
- Brand logos: 200x100px (or similar aspect ratio)
- Category images: 150x150px (square) 