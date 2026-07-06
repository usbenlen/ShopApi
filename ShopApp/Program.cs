using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi;
using Shop.Api.Middleware;
using Shop.Infrastructure.Data;
using Shop.Infrastructure.Repository;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Application.Services;

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
                Title = "Dz6",
                Version = "v1",
                Description = "Dz6 - EFCore"
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            c.IncludeXmlComments(xmlPath);
        });

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
        //builder.Services.AddScoped<IProductService, ProductService>(); // При кожному запиті створюється новий екземпляр об'єкту. (Тобто сервіс не збереже останні POST запити (безпечно)).
        //builder.Services.AddSingleton<IProductService, ProductService>(); // При кожному запиті лишається той самий об'єкт (Сервіс збереже POST запити, для деяких випадків корисно, і потрібно (Дещо небезпечно)).
        //builder.Services.AddSingleton<ICategoryService, CategoryService>();
        //builder.Services.AddSingleton<IUserService, UserService>();

        //Lesson7
        //builder.Services.AddScoped<IProductService, ProductService>();
        builder.Services.AddScoped<ICategoryService, CategoryService>();

        // -- Repositories --
        builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();


        //
        var app = builder.Build();

        // Підключаємо Swagger (тільки в Development mode)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dz6");
            });
        }
        //Дозволити між-доменні запити
        app.UseCors("AllowAll");

        app.MapControllers();
        //app.UseMiddleware<RequestTimerMiddleware>();
        //app.UseMiddleware<UserMiddlewareCheck>();
        //app.UseStaticFiles(); //Доступ до папки wwwroot/images

        app.Run();
    }
}
