using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Shop.Api.Exceptions;
using Shop.Api.Middlewares;
using Shop.Api.Services;
using Shop.Application.Configuration;
using Shop.Application.Interfaces.Helpers;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Application.Mapping;
using Shop.Application.Queries.Product.GetProductById;
using Shop.Application.Services;
using Shop.Application.Services.MessageHandlers;
using Shop.Application.Validators.Category;
using Shop.Infrastructure.Caching;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Helpers;
using Shop.Infrastructure.Repository;
using Shop.Infrastructure.Services;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;

namespace Shop.Api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // -- Database Context --
        builder.Services.AddDbContext<ShopDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        // -- Redis --
        builder.Services.AddSingleton<IConnectionMultiplexer>(_ =>
        {
            var connectionString = builder.Configuration.GetConnectionString("RedisConnection")
                ?? throw new InvalidOperationException("RedisConnection is not configured");

            var options = ConfigurationOptions.Parse(connectionString);

            options.AbortOnConnectFail = false;
            options.ConnectTimeout = 500;
            options.SyncTimeout = 500;
            options.AsyncTimeout = 500;
            options.ConnectRetry = 1;

            return ConnectionMultiplexer.Connect(options);
        });

        // -- Swagger + JWT --
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Lesson8",
                Version = "v1",
                Description = "Lesson8 - EFCore"
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            c.IncludeXmlComments(xmlPath);

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Description = "Enter JWT token"
            });

            c.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });

        // -- RabbitMq --
        builder.Services.AddSingleton<IRabbitMqMessageHandler, UserMessageHandler>();
        builder.Services.AddSingleton<IRabbitMqMessageHandler, OrderMessageHandler>();

        builder.Services.AddHostedService<RabbitMqReaderService>();

        // -- AutoMapper --
        builder.Services.AddAutoMapper(
            _ => { },
            typeof(CategoryProfile).Assembly
        );

        var configuration = builder.Configuration;

        // -- JWT Settings --
        var jwtSettings = builder.Configuration
            .GetSection("JWT")
            .Get<JWTSettings>()
            ?? throw new Exception("JWT settings not configured");

        builder.Services.Configure<JWTSettings>(builder.Configuration.GetSection("JWT"));
        
        // -- Configuration --
        builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("Email"));
        builder.Services.Configure<AdminSeedSettings>(builder.Configuration.GetSection("AdminSeed"));
        builder.Services.Configure<CachingSettings>(builder.Configuration.GetSection("Caching"));
        builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMq"));
        builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDb"));

        // -- Cache Configuration --
        builder.Services
            .AddOptions<CachingSettings>()
            .Bind(builder.Configuration.GetSection("Caching"))
            .Validate(settings =>
                settings.DefaultExpirationMinutes > 0,
                "Caching.DefaultExpirationMinutes must be greater than 0")
            .Validate(settings =>
                settings.Categories.ExpirationMinutes > 0,
                "Caching.Categories.ExpirationMinutes must be greater than 0")
            .Validate(settings =>
                settings.Products.ExpirationMinutes > 0,
                "Caching.Products.ExpirationMinutes must be greater than 0")
            .Validate(settings =>
                settings.L1.ExpirationMinutes > 0,
                "Caching.L1.ExpirationMinutes must be greater than 0")
            .Validate(settings =>
                settings.L1.SizeLimit > 0,
                "Caching.L1.SizeLimit must be greater than 0")
            .Validate(settings =>
                settings.Negative.ExpirationSeconds > 0,
                "Caching.Negative.ExpirationSeconds must be greater than 0")
            .ValidateOnStart();

        // -- CORS (для розробки і білий список) --

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("DevelopmentPolicy", policy =>
            {
                policy.WithOrigins("https://localhost:5173")
                      .AllowAnyMethod()
                      .AllowAnyHeader()
                      .AllowCredentials();
            });
        });


        // -- CORS (З білим списком) --
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("ProductionPolicy", policy =>
            {
                policy.WithOrigins("https://example.com", "https://www.example.com")
                      .WithMethods("GET", "POST", "PUT", "DELETE")
                      .WithHeaders("Content-Type", "Authorization");
            });
        });

        // -- MEDIATR --
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(GetProductByIdHandler).Assembly);
        });

        // -- Validators --
        // Реєструє всі реалізації IValidator<T> зі збірки Shop.Application
        builder.Services.AddValidatorsFromAssembly(typeof(CategoryValidator).Assembly);

        // -- DI container --
        builder.Services.AddControllers();

        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

        // -- Services --
        builder.Services.AddScoped<IProductService, ProductService>(); // При кожному запиті створюється новий екземпляр об'єкту. (Тобто сервіс не збереже останні POST запити (безпечно)).
        //builder.Services.AddSingleton<IProductService, ProductService>(); // При кожному запиті лишається той самий об'єкт (Сервіс збереже POST запити, для деяких випадків корисно, і потрібно (Дещо небезпечно)).
        builder.Services.AddScoped<IProductFeedbackService, MongoProductFeedbackService>();

        builder.Services.AddScoped<IImageService, ImageService>();
        builder.Services.AddScoped<IProductImageService, ProductImageService>();
        builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();

        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IJWTService, JWTService>();
        builder.Services.AddScoped<IRefreshTokenService, RefreshTokenService>();
        builder.Services.AddScoped<IUserService, UserService>();
        builder.Services.AddScoped<IUserAddressService, UserAddressService>();
        builder.Services.AddScoped<IPasswordService, PasswordService>();
        builder.Services.AddScoped<IEmailService, EmailService>();
        builder.Services.AddScoped<IOrderService, OrderService>();

        builder.Services.AddSingleton<IQueueService, RabbitMqService>();
        builder.Services.AddSingleton<IOAuthCodeService, OAuthCodeService>();

        // -- Cache --
        builder.Services.AddSingleton<MemoryCacheStore>();
        builder.Services.AddSingleton<RedisCacheStore>();
        builder.Services.AddSingleton<ICachingService, HybridCachingService>();

        // -- Helpers --
        builder.Services.AddSingleton<IHashHelper, HashHelper>();

        // -- Repositories --
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<IAuthRepository, AuthRepository>();
        builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserAddressRepository, UserAddressRepository>();
        builder.Services.AddScoped<IPasswordResetTokenRepository, PasswordResetTokenRepository>();
        builder.Services.AddScoped<IOrderRepository, OrderRepository>();
        builder.Services.AddScoped<IUserProviderRepository, UserProviderRepository>();
        builder.Services.AddScoped<IProviderRepository, ProviderRepository>();

        // -- Authentication --
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = jwtSettings.Issuer,
                ValidAudience = jwtSettings.Audience,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtSettings.Key)
                ),

                ClockSkew = TimeSpan.Zero
            };
        })
        .AddCookie("External", options =>
        {
            options.Cookie.Name = "shop_external_auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.Lax;

            options.ExpireTimeSpan = TimeSpan.FromMinutes(5);
        })
        .AddGoogle(options =>
        {
            options.ClientId =
                builder.Configuration["Authentication:Google:ClientId"]
                ?? throw new InvalidOperationException("Google ClientId is not configured");

            options.ClientSecret =
                builder.Configuration["Authentication:Google:ClientSecret"]
                ?? throw new InvalidOperationException("Google ClientSecret is not configured");

            options.CallbackPath = "/signin-google";

            options.SignInScheme = "External";

            options.Scope.Add("email");
            options.Scope.Add("profile");

            options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");

            options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "sub");

            options.ClaimActions.MapJsonKey("email_verified", "email_verified");
        });

        builder.Services.AddAuthorization();

        //
        var app = builder.Build();

        // Підключаємо Swagger (тільки в Development mode)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Lesson8");
            });
        }

        //app.UseExceptionHandler();

        //Дозволити між-доменні запити
        app.UseCors("DevelopmentPolicy");
        //app.UseCors("ProductionPolicy"); // Розкоментувати коли підключу свій frontend

        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment()) 
        {
            //app.UseMiddleware<RequestTimerMiddleware>();
            //app.UseMiddleware<UserMiddlewareCheck>();
            app.UseMiddleware<CancellationTokenHandleMiddleware>();
            //app.UseMiddleware<CancellationTestMiddleware>(); Якщо треба протестити CancellationToken
        }

        app.UseStaticFiles(); //Доступ до папки wwwroot/images

        app.MapControllers();

        await AdminSeeder.SeedAsync(app.Services);

        app.Run();
    }
}

//Зробити емаіл і пароль адміна як user-secrets