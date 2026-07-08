using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Shop.Api.Interfaces;
using Shop.Api.Middleware;
using Shop.Api.Services;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Application.Mapping;
using Shop.Application.Services;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repository;

namespace Shop.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // -- Database Context --
        builder.Services.AddDbContext<ShopDbContext>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
        });

        // -- Swagger --
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Dz7",
                Version = "v1",
                Description = "Dz7 - EFCore"
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            c.IncludeXmlComments(xmlPath);
        });

        // -- AutoMapper --
        builder.Services.AddAutoMapper(
            _ => { },
            typeof(CategoryProfile).Assembly
        );

        // -- CORS (Дозволити запити з усіх сайтів до серверу (Але бажано додати білий список)) --
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        // -- DI container --

        builder.Services.AddControllers();

        // -- Services --
        builder.Services.AddScoped<IProductService, ProductService>(); // При кожному запиті створюється новий екземпляр об'єкту. (Тобто сервіс не збереже останні POST запити (безпечно)).
        //builder.Services.AddSingleton<IProductService, ProductService>(); // При кожному запиті лишається той самий об'єкт (Сервіс збереже POST запити, для деяких випадків корисно, і потрібно (Дещо небезпечно)).
        builder.Services.AddScoped<ICategoryService, CategoryService>();
        builder.Services.AddScoped<IImageService, ImageService>();

        // -- Repositories --
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();


        //
        var app = builder.Build();

        // Підключаємо Swagger (тільки в Development mode)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dz7");
            });
        }
        //Дозволити між-доменні запити
        app.UseCors("AllowAll");

        //app.UseMiddleware<RequestTimerMiddleware>();
        //app.UseMiddleware<UserMiddlewareCheck>();
        app.UseStaticFiles(); //Доступ до папки wwwroot/images

        app.MapControllers();

        app.Run();
    }
}
