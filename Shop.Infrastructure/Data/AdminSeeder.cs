using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Shop.Domain.Enums;
using Shop.Infrastructure.Configuration;
using Shop.Application.Interfaces.Helpers;

namespace Shop.Infrastructure.Data;

public static class AdminSeeder
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<ShopDbContext>();

        var settings = scope.ServiceProvider.GetRequiredService<IOptions<AdminSeedSettings>>().Value;

        if (string.IsNullOrWhiteSpace(settings.Email) || string.IsNullOrWhiteSpace(settings.Password))
            return;

        var adminExists = await context.Users.AnyAsync(x => x.Role == UserRole.Admin);

        if (adminExists) return;

        var hashHelper = scope.ServiceProvider.GetRequiredService<IHashHelper>();

        var admin = new User
        {
            Email = settings.Email.Trim().ToLowerInvariant(),
            PasswordHash = hashHelper.Hash(settings.Password),
            Role = UserRole.Admin,
            IsActive = true
        };

        await context.Users.AddAsync(admin);

        await context.SaveChangesAsync();
    }
}