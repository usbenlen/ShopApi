using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Shop.Application.Interfaces.Services;
using System.Collections.Concurrent;

namespace Shop.Infrastructure.Services;

public sealed class MemoryCachingService(IMemoryCache _memoryCache, ILogger<MemoryCachingService> _logger) : ICachingService
{
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
    private readonly ConcurrentDictionary<string, CancellationTokenSource> _groups = new();

    public Task<T?> GetAsync<T>(string key)
    {
        return Task.FromResult(
            _memoryCache.TryGetValue(key, out T? value)
                ? value : default);
    }

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan expiration, string? group = null)
    {
        if (_memoryCache.TryGetValue(key, out T? cached)) 
        {
            _logger.LogDebug("Cache HIT: {CacheKey}",key);
            return cached; 
        }

        _logger.LogDebug("Cache MISS: {CacheKey}", key);

        var semaphore = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();

        try
        {
            // Перевіряєм чи інший request вже заповнив cache
            if (_memoryCache.TryGetValue(key, out cached))
            {
                _logger.LogDebug("Cache HIT after lock: {CacheKey}", key);
                return cached;
            }

            var value = await factory();

            if (value is null) return default;

            var options = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration,
                Priority = CacheItemPriority.Normal
            };

            if (group is not null)
            {
                var groupToken = GetGroupToken(group);

                options.AddExpirationToken(new CancellationChangeToken(groupToken));
            }

            _memoryCache.Set(key, value, options);

            _logger.LogDebug("Cache SET: {CacheKey}", key);

            return value;
        }
        finally
        {
            semaphore.Release();
            _locks.TryRemove(key, out _);
        }
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, string? group = null)
    {
        var options = new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(15),
            Priority = CacheItemPriority.Normal
        };

        if (group is not null)
        {
            var groupToken = GetGroupToken(group);

            options.AddExpirationToken(new CancellationChangeToken(groupToken));
        }

        _memoryCache.Set(key, value, options);

        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        _memoryCache.Remove(key);

        return Task.CompletedTask;
    }

    public Task InvalidateGroupAsync(string group)
    {
        if (!_groups.TryGetValue(group, out var cancellationTokenSource))
            return Task.CompletedTask;

        _groups.TryRemove(new KeyValuePair<string, CancellationTokenSource>(group, cancellationTokenSource));

        cancellationTokenSource.Cancel();
        cancellationTokenSource.Dispose();

        return Task.CompletedTask;
    }

    private CancellationToken GetGroupToken(string group)
    {
        var cancellationTokenSource = _groups.GetOrAdd(group, _ => new CancellationTokenSource());
        return cancellationTokenSource.Token;
    }
}
