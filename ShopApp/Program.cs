using ShopApp.Interfaces;
using ShopApp.Services;

namespace ShopApp;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddSingleton<IProductService, ProductService>();

        var app = builder.Build();
        app.MapControllers();
        app.Run();
    }
}
