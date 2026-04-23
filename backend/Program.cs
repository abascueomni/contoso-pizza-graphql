using System.Text;
using ContosoPizza.Data;
using ContosoPizza.GraphQL;
using ContosoPizza.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Azure.Identity;
using Azure.Extensions.AspNetCore.Configuration.Secrets;
using System.Security.Cryptography;


var builder = WebApplication.CreateBuilder(args);

// Access keys from the Azure Key Vault
var keyVaultUrl = builder.Configuration["KeyVault:Url"];

if (!string.IsNullOrEmpty(keyVaultUrl))
{
    builder.Configuration.AddAzureKeyVault(
        new Uri(keyVaultUrl),
        new DefaultAzureCredential()
    );
}

// ---------------------------
// Add services to the container
// ---------------------------

// Add controllers with global JWT authorization and JSON enum support
builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter(policy));
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
});

// Register DbContext
var connectionString = builder.Configuration["ConnectionStrings:DefaultConnection"];
builder.Services.AddDbContext<PizzaContext>(options =>
    options.UseSqlServer(connectionString));



//Add Api versioning    
builder.Services.AddApiVersioning(options =>
{
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
});

// Swagger with JWT support
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV"; // e.g., v1, v2
    options.SubstituteApiVersionInUrl = true;
});
var provider = builder.Services.BuildServiceProvider()
                  .GetRequiredService<IApiVersionDescriptionProvider>();
builder.Services.AddSwaggerGen(c =>
{

    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerDoc(description.GroupName, new OpenApiInfo
        {
            Title = $"Pizza API {description.ApiVersion}",
            Version = description.ApiVersion.ToString()
        });
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])) // stored in user secrets
    };
});

builder.Services.AddAuthorization();
//add ability to use distributed memory cache
builder.Services.AddDistributedMemoryCache();

//register resolvers
builder.Services.AddScoped<Query>();
builder.Services.AddScoped<Mutation>();

//Adding GraphQL support
builder.Services
    .AddGraphQLServer()
    .AddAuthorization()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddFiltering();

// ---------------------------
// Add CORS
// ---------------------------
var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        if (allowedOrigins != null && allowedOrigins.Length > 0)
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});


// ---------------------------
// Build app
// ---------------------------
var app = builder.Build();

// ---------------------------
// Configure middleware
// ---------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
{
    foreach (var description in provider.ApiVersionDescriptions)
    {
        c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
            description.GroupName.ToUpperInvariant());
    }
});
}

//app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

// Order matters: Authentication first, then Authorization
app.UseAuthentication();
app.UseAuthorization();
app.MapGraphQL("/gql");
app.MapControllers();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<PizzaContext>();
    // Seed pizzas if none exist
    if (!db.Pizzas.Any())
    {
        var classic = new Pizza
        {
            Name = "Classic Italian",
            Price = 5.00,
            IsGlutenFree = false,
            IsMenuPizza = true,
            Toppings = new List<PizzaTopping>
        {
            new PizzaTopping { Topping = Topping.Cheese },
            new PizzaTopping { Topping = Topping.Tomato }
        }
        };

        var veggie = new Pizza
        {
            Name = "Veggie",
            Price = 5.00,
            IsGlutenFree = true,
            IsMenuPizza = true,
            Toppings = new List<PizzaTopping>
            {
                new PizzaTopping { Topping = Topping.Tomato },
                new PizzaTopping { Topping = Topping.Peppers },
                new PizzaTopping { Topping = Topping.Mushrooms }
            }
        };

        var pepperoni = new Pizza
        {
            Name = "Pepperoni",
            Price = 5.00,
            IsGlutenFree = false,
            IsMenuPizza = true,
            Toppings = new List<PizzaTopping>
            {
                new PizzaTopping { Topping = Topping.Cheese },
                new PizzaTopping { Topping = Topping.Tomato },
                new PizzaTopping { Topping = Topping.Pepperoni }
            }
        };
        var hawaiian = new Pizza
        {
            Name = "Hawaiian",
            Price = 6.00,
            IsGlutenFree = false,
            IsMenuPizza = true,
            Toppings = new List<PizzaTopping>
            {
                new PizzaTopping { Topping = Topping.Cheese },
                new PizzaTopping { Topping = Topping.Ham },
                new PizzaTopping { Topping = Topping.Pineapple }
            }
        };

        // Add pizzas to the context and save to generate IDs
        db.Pizzas.AddRange(classic, veggie, pepperoni, hawaiian);
        db.SaveChanges();

        // Fetch tracked entities from the context to use in orders
        var classicDb = db.Pizzas.First(p => p.Name == "Classic Italian");
        var veggieDb = db.Pizzas.First(p => p.Name == "Veggie");
        var hawaiianDb = db.Pizzas.First(p => p.Name == "Hawaiian");
        // Seed a sample order using tracked pizzas
        if (!db.Orders.Any())
        {
            var sampleOrder1 = new Order
            {
                CustomerName = "Jim",
                CreatedAt = DateTime.UtcNow,
                PickUpTime = DateTime.UtcNow.AddMinutes(20),
                Pizzas = new List<OrderPizza>
                {
                    new OrderPizza { Pizza = classicDb, Quantity = 1 },
                    new OrderPizza { Pizza = veggieDb, Quantity = 1 }
                }
            };
            var sampleOrder2 = new Order
            {
                CustomerName = "Kali the Hawaiian Enjoyer",
                CreatedAt = DateTime.UtcNow,
                PickUpTime = DateTime.UtcNow.AddMinutes(40),
                Pizzas = new List<OrderPizza>
                {
                    new OrderPizza { Pizza = hawaiianDb, Quantity = 4 },
                }
            };

            db.Orders.AddRange(sampleOrder1, sampleOrder2);
            db.SaveChanges(); // Order ID and join table populated
        }
    }
    // Seed Coupons if none exist
    if (!db.Coupons.Any())
    {
        db.Coupons.Add(new Coupon
        {
            CouponCode = "WELCOME10",
            DiscountPercent = 0.10, // 10% off
        });

        db.SaveChanges();
    }

}



app.Run();
