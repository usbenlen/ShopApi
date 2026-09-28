using Shop.Application.Interfaces.Services;
using StackExchange.Redis;
using System.Security.Cryptography;

namespace Shop.Infrastructure.Services;

public sealed class OAuthCodeService(IConnectionMultiplexer redis) : IOAuthCodeService
{
    private const string KeyPrefix = "shop:oauth:code:";

    private readonly IDatabase _database = redis.GetDatabase();

    private static readonly TimeSpan CodeLifetime = TimeSpan.FromMinutes(1);

    public async Task<string> CreateAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var bytes = RandomNumberGenerator.GetBytes(32);

        var code = Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        var key = $"{KeyPrefix}{code}";

        await _database.StringSetAsync(key, accessToken, CodeLifetime);

        return code;
    }

    public async Task<string?> ConsumeAsync(string code, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(code)) return null;

        var key = $"{KeyPrefix}{code}";

        // GETDEL - отримати значення і одразу видалити
        var value = await _database.StringGetDeleteAsync(key);

        if (value.IsNullOrEmpty) return null;

        return value.ToString();
    }
}
