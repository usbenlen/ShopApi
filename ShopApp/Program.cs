using ShopApp.Interfaces;
using ShopApp.Middleware;
using ShopApp.Services;

namespace ShopApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        //builder.Services.AddScoped<IProductService, ProductService>(); // При кожному запиті створюється новий екземпляр об'єкту. (Тобто сервіс не збереже останні POST запити (безпечно)).
        builder.Services.AddSingleton<IProductService, ProductService>(); // При кожному запиті лишається той самий об'єкт (Сервіс збереже POST запити, для деяких випадків корисно, і потрібно (Дещо небезпечно)).
        builder.Services.AddSingleton<ICategoryService, CategoryService>();
        builder.Services.AddSingleton<IUserService, UserService>();

        var app = builder.Build();
        app.MapControllers();
        //app.UseMiddleware<RequestTimerMiddleware>();
        app.UseMiddleware<UserMiddlewareCheck>();
        app.Run();
    }
}
