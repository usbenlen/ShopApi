using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Application.Configuration;
using Shop.Application.Interfaces.Cache;
using System.Collections.Concurrent;

namespace Shop.Infrastructure.Caching;

public sealed class MemoryCacheStore : ICacheStore, IDisposable
{
    private readonly MemoryCache _cache;
    private readonly CachingSettings _settings;
    private readonly ILogger<MemoryCacheStore> _logger;

    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _groups = new();

    public MemoryCacheStore(IOptions<CachingSettings> options, ILogger<MemoryCacheStore> logger)
    {
        _settings = options.Value;
        _logger = logger;

        _cache = new MemoryCache(
            new MemoryCacheOptions
            {
                SizeLimit = _settings.L1.SizeLimit
            });
    }

    public Task<CacheLookupResult<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_cache.TryGetValue(key, out MemoryCacheEntry? entry))
        {
            _logger.LogDebug("L1 MISS: {CacheKey}", key);

            return Task.FromResult(CacheLookupResult<T>.Miss());
        }

        if (entry is null)
        {
            _logger.LogWarning("L1 cache returned a null entry. Key: {CacheKey}", key);

            return Task.FromResult(CacheLookupResult<T>.Miss());
        }

        if (entry.IsNegative)
        {
            _logger.LogDebug("L1 NEGATIVE HIT: {CacheKey}", key);

            return Task.FromResult(CacheLookupResult<T>.Hit(default));
        }

        if (entry.Value is T value)
        {
            _logger.LogDebug("L1 HIT: {CacheKey}", key);

            return Task.FromResult(CacheLookupResult<T>.Hit(value));
        }

        // Якщо в cache пошкоджені дані, то краще видалити
        _cache.Remove(key);

        _logger.LogWarning("L1 cache entry has unexpected type. Key: {CacheKey}", key);

        return Task.FromResult(CacheLookupResult<T>.Miss());
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan expiration, string? group = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var options = CreateOptions(expiration, group);

        _cache.Set(key, MemoryCacheEntry.FromValue(value), options);

        _logger.LogDebug("L1 SET: {CacheKey}", key);

        await Task.CompletedTask;
    }

    public async Task SetNegativeAsync(string key, TimeSpan expiration, string? group = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var options = CreateOptions(expiration, group);

        _cache.Set(key, MemoryCacheEntry.Negative(), options);

        _logger.LogDebug("L1 NEGATIVE SET: {CacheKey}", key);

        await Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        _cache.Remove(key);

        return Task.CompletedTask;
    }

    public Task InvalidateGroupAsync(string group, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!_groups.TryRemove(group, out var source))
            return Task.CompletedTask;

        try
        {
            source.Cancel();
        }
        finally
        {
            source.Dispose();
        }

        _logger.LogDebug("L1 GROUP INVALIDATED: {CacheGroup}", group);

        return Task.CompletedTask;
    }

    private MemoryCacheEntryOptions CreateOptions(TimeSpan expiration, string? group)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration,
            Priority = CacheItemPriority.Normal,

            // SizeLimit = кількість записів
            Size = 1
        };

        if (group is not null)
        {
            var token = GetGroupToken(group);

            options.AddExpirationToken(new Microsoft.Extensions.Primitives.CancellationChangeToken(token));
        }

        return options;
    }

    private CancellationToken GetGroupToken(string group)
    {
        var source = _groups.GetOrAdd(group, static _ => new CancellationTokenSource());
        return source.Token;
    }

    public void Dispose()
    {
        _cache.Dispose();

        foreach (var source in _groups.Values) source.Dispose();
        foreach (var semaphore in _locks.Values) semaphore.Dispose();

        _groups.Clear();
        _locks.Clear();
    }

    private sealed class MemoryCacheEntry
    {
        public object? Value { get; }
        public bool IsNegative { get; }

        private MemoryCacheEntry(object? value, bool isNegative)
        {
            Value = value;
            IsNegative = isNegative;
        }

        public static MemoryCacheEntry FromValue<T>(T value) => new(value, false);
        public static MemoryCacheEntry Negative() => new(null, true);
    }
}