namespace Shop.Application.Configuration;

public class CachingSettings
{
    public int DefaultExpirationMinutes { get; set; }

    public CacheEntitySettings Categories { get; set; } = new();
    public CacheEntitySettings Products { get; set; } = new();

    public L1CacheSettings L1 { get; set; } = new();
    public NegativeCacheSettings Negative { get; set; } = new();
}

public class CacheEntitySettings
{
    public int ExpirationMinutes { get; set; }
}

public sealed class L1CacheSettings
{
    public int ExpirationMinutes { get; set; } = 5;

    // Максимальна кількість записів у L1
    public int SizeLimit { get; set; } = 10_000;
}

public sealed class NegativeCacheSettings
{
    public int ExpirationSeconds { get; set; } = 30;
}