using System.Text.Json;

namespace Shop.Infrastructure.Caching;

internal sealed class RedisCacheEntry
{
    public bool IsNegative { get; init; }
    public JsonElement? Value { get; init; }
    public string? Group { get; init; }
    public long GroupVersion { get; init; }
}