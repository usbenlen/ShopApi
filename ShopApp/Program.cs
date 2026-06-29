using Microsoft.OpenApi;
using ShopApp.Interfaces;
using ShopApp.Middleware;
using ShopApp.Services;

namespace ShopApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Dz2",
                Version = "v1",
                Description = "Task Dz2"
            });

            var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            c.IncludeXmlComments(xmlPath);
        });

        builder.Services.AddControllers();
        //builder.Services.AddScoped<IProductService, ProductService>(); // При кожному запиті створюється новий екземпляр об'єкту. (Тобто сервіс не збереже останні POST запити (безпечно)).
        builder.Services.AddSingleton<IProductService, ProductService>(); // При кожному запиті лишається той самий об'єкт (Сервіс збереже POST запити, для деяких випадків корисно, і потрібно (Дещо небезпечно)).
        //builder.Services.AddSingleton<ICategoryService, CategoryService>();
        //builder.Services.AddSingleton<IUserService, UserService>();

        var app = builder.Build();

        // Підключаємо Swagger (тільки в Development mode)
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dz2");
            });
        }

        app.MapControllers();
        //app.UseMiddleware<RequestTimerMiddleware>();
        //app.UseMiddleware<UserMiddlewareCheck>();
        //app.UseStaticFiles(); //Доступ до папки wwwroot/images

        app.Run();
    }
}
