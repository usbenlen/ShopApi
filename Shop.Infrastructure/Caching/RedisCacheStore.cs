using Microsoft.Extensions.Logging;
using Shop.Application.Interfaces.Cache;
using StackExchange.Redis;
using System.Text.Json;

namespace Shop.Infrastructure.Caching;

public sealed class RedisCacheStore : ICacheStore
{
    private const string KeyPrefix = "shop:cache:v1:";
    private const string GroupPrefix = "shop:cache:group:v1:";

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IDatabase _database;
    private readonly ILogger<RedisCacheStore> _logger;

    public RedisCacheStore(IConnectionMultiplexer multiplexer, ILogger<RedisCacheStore> logger)
    {
        _database = multiplexer.GetDatabase();
        _logger = logger;
    }

    public async Task<CacheLookupResult<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var redisKey = BuildKey(key);
            var value = await _database.StringGetAsync(redisKey);

            if (value.IsNullOrEmpty)
            {
                _logger.LogDebug("L2 MISS: {CacheKey}", key);

                return CacheLookupResult<T>.Miss();
            }

            RedisCacheEntry? entry;

            try
            {
                entry = JsonSerializer.Deserialize<RedisCacheEntry>((string)value!, JsonOptions);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Invalid Redis cache payload. Key: {CacheKey}", key);

                await SafeDeleteAsync(redisKey);
                return CacheLookupResult<T>.Miss();
            }

            if (entry is null)
            {
                await SafeDeleteAsync(redisKey);
                return CacheLookupResult<T>.Miss();
            }

            if (entry.Group is not null)
            {
                var currentVersion = await GetGroupVersionAsync(entry.Group);

                if (currentVersion != entry.GroupVersion)
                {
                    _logger.LogDebug("L2 STALE: {CacheKey}, Group: {CacheGroup}", key, entry.Group);

                    await SafeDeleteAsync(redisKey);
                    return CacheLookupResult<T>.Miss();
                }
            }

            if (entry.IsNegative)
            {
                _logger.LogDebug("L2 NEGATIVE HIT: {CacheKey}", key);
                return CacheLookupResult<T>.Hit(default);
            }

            if (entry.Value is null)
            {
                await SafeDeleteAsync(redisKey);
                return CacheLookupResult<T>.Miss();
            }

            try
            {
                var result = entry.Value.Value.Deserialize<T>(JsonOptions);

                _logger.LogDebug("L2 HIT: {CacheKey}", key);
                return CacheLookupResult<T>.Hit(result);
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Failed to deserialize Redis cache value. Key: {CacheKey}", key);

                await SafeDeleteAsync(redisKey);
                return CacheLookupResult<T>.Miss();
            }
        }
        catch (RedisException ex)
        {
            _logger.LogWarning(ex, "Redis unavailable during GET. Key: {CacheKey}", key);

            return CacheLookupResult<T>.Unavailable();
        }
        catch (TimeoutException ex)
        {
            _logger.LogWarning(ex, "Redis timeout during GET. Key: {CacheKey}", key);

            return CacheLookupResult<T>.Unavailable();
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, string? group = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var groupVersion = group is null ? 0 : await GetGroupVersionAsync(group);

            var serializedValue = JsonSerializer.SerializeToElement(value, JsonOptions);

            var entry = new RedisCacheEntry
            {
                IsNegative = false,
                Value = serializedValue,
                Group = group,
                GroupVersion = groupVersion
            };

            var json = JsonSerializer.Serialize(entry, JsonOptions);

            await _database.StringSetAsync(BuildKey(key), json, expiration);

            _logger.LogDebug("L2 SET: {CacheKey}", key);
        }
        catch (RedisException ex)
        {
            _logger.LogWarning(ex, "Redis unavailable during SET. Key: {CacheKey}", key);
        }
        catch (TimeoutException ex)
        {
            _logger.LogWarning(ex, "Redis timeout during SET. Key: {CacheKey}", key);
        }
    }

    public async Task SetNegativeAsync(string key, TimeSpan expiration, string? group = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var groupVersion = group is null ? 0 : await GetGroupVersionAsync(group);

            var entry = new RedisCacheEntry
            {
                IsNegative = true,
                Value = null,
                Group = group,
                GroupVersion = groupVersion
            };

            var json = JsonSerializer.Serialize(entry, JsonOptions);

            await _database.StringSetAsync(BuildKey(key), json, expiration);

            _logger.LogDebug("L2 NEGATIVE SET: {CacheKey}", key);
        }
        catch (RedisException ex)
        {
            _logger.LogWarning(ex, "Redis unavailable during negative SET. Key: {CacheKey}", key);
        }
        catch (TimeoutException ex)
        {
            _logger.LogWarning(ex, "Redis timeout during negative SET. Key: {CacheKey}", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await _database.KeyDeleteAsync(BuildKey(key));
        }
        catch (RedisException ex)
        {
            _logger.LogWarning(ex, "Redis unavailable during REMOVE. Key: {CacheKey}", key);
        }
        catch (TimeoutException ex)
        {
            _logger.LogWarning(ex, "Redis timeout during REMOVE. Key: {CacheKey}", key);
        }
    }

    public async Task InvalidateGroupAsync(string group, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            await _database.StringIncrementAsync(BuildGroupKey(group));

            _logger.LogDebug("L2 GROUP INVALIDATED: {CacheGroup}", group);
        }
        catch (RedisException ex)
        {
            _logger.LogWarning(ex, "Redis unavailable during group invalidation. Group: {CacheGroup}", group);
        }
        catch (TimeoutException ex)
        {
            _logger.LogWarning(ex, "Redis timeout during group invalidation. Group: {CacheGroup}", group);
        }
    }

    private async Task<long> GetGroupVersionAsync(string group)
    {
        var value = await _database.StringGetAsync(BuildGroupKey(group));
        if (!value.HasValue) return 0;

        return (long)value;
    }

    private async Task SafeDeleteAsync(RedisKey key)
    {
        try
        {
            await _database.KeyDeleteAsync(key);
        }
        catch (RedisException ex)
        {
            _logger.LogDebug(ex, "Failed to delete invalid Redis cache entry: {CacheKey}", key);
        }
        catch (TimeoutException ex)
        {
            _logger.LogDebug(ex, "Timeout deleting invalid Redis cache entry: {CacheKey}", key);
        }
    }

    private static RedisKey BuildKey(string key) => $"{KeyPrefix}{key}";
    private static RedisKey BuildGroupKey(string group) => $"{GroupPrefix}{group}";
}