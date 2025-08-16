using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EcommerceAPI.Data;
using EcommerceAPI.Services;
using EcommerceAPI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure SQLite Database
builder.Services.AddDbContext<EcommerceDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    
    // For production on Azure Linux App Service, ensure database is in writable location
    if (builder.Environment.IsProduction())
    {
        if (string.IsNullOrEmpty(connectionString) || !connectionString.Contains("/home/"))
        {
            connectionString = "Data Source=/home/ecommerce.db";
        }
        
        // Ensure the directory exists
        if (!string.IsNullOrEmpty(connectionString))
        {
            var dbPath = connectionString.Replace("Data Source=", "");
            var directory = Path.GetDirectoryName(dbPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }
    }
    
    options.UseSqlite(connectionString);
});

// Configure AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Configure JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "DefaultSecretKey"))
        };
    });

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp",
        policy =>
        {
            var corsOrigins = builder.Configuration.GetSection("CorsOrigins").Get<string[]>();
            if (corsOrigins != null && corsOrigins.Length > 0)
            {
                policy.WithOrigins(corsOrigins);
            }
            else
            {
                // Default to localhost for development
                policy.WithOrigins("http://localhost:4200");
            }
            
            policy.AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

// Register Services
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IBrandService, BrandService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFrequentlyBoughtTogetherService, FrequentlyBoughtTogetherService>();
builder.Services.AddScoped<IStripeService, StripeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // Enable Swagger in Production for testing (remove this after testing!)
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EcommerceAPI v1");
        c.RoutePrefix = "api/swagger"; // Serve Swagger UI at /api/swagger
    });
}

app.UseHttpsRedirection();

// Configure static files to serve Angular app
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors("AllowAngularApp");

app.UseAuthentication();
app.UseAuthorization();

// Map API controllers
app.MapControllers();

// Add fallback to index.html for Angular routing
app.MapFallbackToFile("index.html");

// Ensure database is created and up to date
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EcommerceDbContext>();
    dbContext.Database.EnsureCreated();
    
    // Initialize database with custom tables and seed data
    DatabaseInitializer.Initialize(dbContext);
}

app.Run(); 