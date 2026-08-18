namespace Shop.Application.Configuration;

public class CachingSettings
{
    public int DefaultExpirationMinutes { get; set; }
    public CacheEntitySettings Categories { get; set; } = new();
    public CacheEntitySettings Products { get; set; } = new();
}

public class CacheEntitySettings
{
    public int ExpirationMinutes { get; set; }
}