using AutoMapper;
using EcommerceAPI.Models;
using EcommerceAPI.DTOs;

namespace EcommerceAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // User mappings
            CreateMap<User, UserDto>();
            CreateMap<UserRegisterDto, User>();
            CreateMap<UserUpdateDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.SubcategoryName, opt => opt.MapFrom(src => src.Subcategory != null ? src.Subcategory.Name : null))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : (double?)null))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count));
            
            CreateMap<Product, ProductListDto>()
                .ForMember(dest => dest.BrandName, opt => opt.MapFrom(src => src.Brand != null ? src.Brand.Name : null))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : (double?)null))
                .ForMember(dest => dest.ReviewCount, opt => opt.MapFrom(src => src.Reviews.Count));
            
            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            // Product Variant mappings
            CreateMap<ProductVariant, ProductVariantDto>();
            CreateMap<ProductVariantDto, ProductVariant>();

            // Product Tag mappings
            CreateMap<ProductTag, ProductTagDto>();
            CreateMap<ProductTagDto, ProductTag>();

            // Category mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ParentName, opt => opt.MapFrom(src => src.Parent != null ? src.Parent.Name : null))
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));
            
            CreateMap<Category, CategoryListDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count))
                .ForMember(dest => dest.Subcategories, opt => opt.MapFrom(src => src.Subcategories));
            
            CreateMap<CategoryCreateDto, Category>();

            // Subcategory mappings
            CreateMap<Subcategory, SubcategoryDto>();
            CreateMap<Subcategory, SubcategoryListDto>();
            CreateMap<SubcategoryCreateDto, Subcategory>();

            // Brand mappings
            CreateMap<Brand, BrandDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products.Count));

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()));
            CreateMap<Order, OrderListDto>()
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaymentStatus, opt => opt.MapFrom(src => src.PaymentStatus.ToString()))
                .ForMember(dest => dest.ItemCount, opt => opt.MapFrom(src => src.OrderItems.Count));
            CreateMap<CreateOrderDto, Order>();

            // Order Item mappings
            CreateMap<OrderItem, OrderItemDto>();
            
            // Order Status History mappings
            CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User != null ? $"{src.User.FirstName} {src.User.LastName}".Trim() : null));

            // Cart Item mappings
            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
                .ForMember(dest => dest.VariantName, opt => opt.MapFrom(src => src.Variant != null ? src.Variant.VariantName : null))
                .ForMember(dest => dest.ItemTotal, opt => opt.MapFrom(src => src.Quantity * (src.Product.Price + (src.Variant != null ? src.Variant.PriceAdjustment : 0))));

            // Wishlist Item mappings
            CreateMap<WishlistItem, WishlistItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price));

            // Review mappings
            CreateMap<ProductReview, ReviewDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}".Trim()));
            CreateMap<CreateReviewDto, ProductReview>();

            // Address mappings
            CreateMap<UserAddress, UserAddressDto>();
            CreateMap<CreateAddressDto, UserAddress>();

            // Currency mappings
            CreateMap<Currency, CurrencyDto>();

            // Language mappings
            CreateMap<Language, LanguageDto>();

            // FrequentlyBoughtTogether mappings
            CreateMap<FrequentlyBoughtTogether, FrequentlyBoughtTogetherDto>()
                .ForMember(dest => dest.RelatedProduct, opt => opt.MapFrom(src => src.RelatedProduct));
        }
    }
} 