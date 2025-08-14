# E-commerce Web API

A comprehensive .NET Core 8.0 Web API for an e-commerce platform using SQLite database.

## Features

- **Authentication & Authorization**: JWT-based authentication with role-based access control
- **Product Management**: CRUD operations for products, categories, brands
- **Shopping Cart**: Add, update, remove items from cart
- **Order Management**: Create orders, track order status, order history
- **User Management**: Profile management, address management
- **Search & Filter**: Product search with pagination and filtering
- **RESTful API**: Well-structured REST endpoints with consistent responses

## Prerequisites

- .NET 8.0 SDK
- Visual Studio 2022 or VS Code
- SQLite

## Getting Started

1. **Navigate to the WebAPI directory**:
   ```bash
   cd C:\Users\2325185\code_base\e-commerce-app\WebAPI
   ```

2. **Restore NuGet packages**:
   ```bash
   dotnet restore
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

4. **Access Swagger UI**:
   Open your browser and navigate to: `http://localhost:5000/swagger`

## API Endpoints

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration
- `POST /api/auth/change-password` - Change password (requires authentication)

### Products
- `GET /api/products` - Get all products (with pagination)
- `GET /api/products/{id}` - Get product by ID
- `GET /api/products/slug/{slug}` - Get product by slug
- `GET /api/products/featured` - Get featured products
- `GET /api/products/new` - Get new products
- `GET /api/products/on-sale` - Get products on sale
- `GET /api/products/{id}/related` - Get related products
- `POST /api/products` - Create product (Admin only)
- `PUT /api/products/{id}` - Update product (Admin only)
- `DELETE /api/products/{id}` - Delete product (Admin only)

### Categories
- `GET /api/categories` - Get all categories
- `GET /api/categories/top` - Get top categories
- `GET /api/categories/{id}` - Get category by ID
- `GET /api/categories/slug/{slug}` - Get category by slug
- `GET /api/categories/{id}/subcategories` - Get subcategories
- `POST /api/categories` - Create category (Admin only)
- `PUT /api/categories/{id}` - Update category (Admin only)
- `DELETE /api/categories/{id}` - Delete category (Admin only)

### Cart (Requires Authentication)
- `GET /api/cart` - Get user's cart
- `POST /api/cart/items` - Add item to cart
- `PUT /api/cart/items` - Update cart item quantity
- `DELETE /api/cart/items/{id}` - Remove item from cart
- `DELETE /api/cart` - Clear cart
- `GET /api/cart/count` - Get cart item count

### Orders (Requires Authentication)
- `GET /api/orders` - Get user's orders
- `GET /api/orders/{id}` - Get order by ID
- `POST /api/orders` - Create order
- `PUT /api/orders/{id}/cancel` - Cancel order
- `GET /api/orders/all` - Get all orders (Admin only)
- `PUT /api/orders/{id}/status` - Update order status (Admin only)

### Users (Requires Authentication)
- `GET /api/users/profile` - Get user profile
- `PUT /api/users/profile` - Update user profile
- `GET /api/users/addresses` - Get user addresses
- `POST /api/users/addresses` - Create address
- `PUT /api/users/addresses/{id}` - Update address
- `DELETE /api/users/addresses/{id}` - Delete address
- `PUT /api/users/addresses/{id}/default` - Set default address

### Brands
- `GET /api/brands` - Get all brands
- `GET /api/brands/featured` - Get featured brands
- `GET /api/brands/{id}` - Get brand by ID
- `POST /api/brands` - Create brand (Admin only)
- `PUT /api/brands/{id}` - Update brand (Admin only)
- `DELETE /api/brands/{id}` - Delete brand (Admin only)

## Database

The API uses SQLite database with the following main tables:
- Users
- Products
- Categories & Subcategories
- Brands
- Orders & OrderItems
- Cart Items
- Wishlist Items
- Product Reviews
- User Addresses

## Authentication

The API uses JWT tokens for authentication. To access protected endpoints:

1. Register or login to get a JWT token
2. Include the token in the Authorization header: `Bearer {token}`

## Response Format

All API responses follow a consistent format:

```json
{
  "success": true,
  "message": "Operation successful",
  "data": { ... },
  "errors": []
}
```

## Pagination

List endpoints support pagination with the following query parameters:
- `pageNumber` - Page number (default: 1)
- `pageSize` - Items per page (default: 10, max: 50)
- `sortBy` - Sort field
- `sortDescending` - Sort order (default: false)
- `searchTerm` - Search term

## Development

The project structure follows clean architecture principles:
- **Controllers**: API endpoints
- **Services**: Business logic
- **Models**: Entity models
- **DTOs**: Data transfer objects
- **Data**: Entity Framework DbContext and configurations
- **Mappings**: AutoMapper profiles

## Configuration

Key configuration settings in `appsettings.json`:
- Connection string for SQLite database
- JWT settings (key, issuer, audience, expiration)
- CORS policy for Angular frontend

## Testing

Use the Swagger UI at `http://localhost:5000/swagger` to test API endpoints interactively.

## License

This project is part of an e-commerce application demo. 