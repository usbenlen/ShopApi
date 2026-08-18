using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Shop.Application.Configuration;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Caching;
using System.Collections.Concurrent;

namespace Shop.Infrastructure.Services;

public sealed class HybridCachingService(MemoryCacheStore _l1, RedisCacheStore _l2, IOptions<CachingSettings> _options, ILogger<HybridCachingService> _logger) : ICachingService
{
    private readonly CachingSettings _settings = _options.Value;
    private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    public async Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan expiration, string? group = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var negativeExpiration = TimeSpan.FromSeconds(_settings.Negative.ExpirationSeconds);
        var l1Expiration = GetL1Expiration(expiration);

        // -- L1 --

        var l1Result = await _l1.GetAsync<T>(key, cancellationToken);

        if (l1Result.IsHit)
        {
            _logger.LogDebug("CACHE HIT L1: {CacheKey}", key);
            return l1Result.Value;
        }

        // -- L2 --

        var l2Result = await _l2.GetAsync<T>(key, cancellationToken);

        if (l2Result.IsHit)
        {
            _logger.LogDebug("CACHE HIT L2: {CacheKey}", key);

            if (l2Result.Value is null)
                await _l1.SetNegativeAsync(key, negativeExpiration, group, cancellationToken);
            else
                await _l1.SetAsync(key, l2Result.Value, l1Expiration, group, cancellationToken);

            return l2Result.Value;
        }

        // -- L1 + L2 MISS --

        var semaphore = _locks.GetOrAdd(key, static _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);

        try
        {
            // перевіряєм чи інший запит вже заповнив l1
            l1Result = await _l1.GetAsync<T>(key, cancellationToken);

            if (l1Result.IsHit) return l1Result.Value;

            // перевіряєм чи інший запит вже заповнив l2
            l2Result = await _l2.GetAsync<T>(key, cancellationToken);

            if (l2Result.IsHit)
            {
                if (l2Result.Value is null)
                    await _l1.SetNegativeAsync(key, negativeExpiration, group, cancellationToken);
                else
                    await _l1.SetAsync(key, l2Result.Value, l1Expiration, group, cancellationToken);

                return l2Result.Value;
            }

            // бд
            _logger.LogDebug("CACHE MISS -> FACTORY: {CacheKey}", key);

            var value = await factory();

            // -- Negative cache --

            if (value is null)
            {
                await _l1.SetNegativeAsync(key, negativeExpiration, group, cancellationToken);
                await _l2.SetNegativeAsync(key, negativeExpiration, group, cancellationToken);

                _logger.LogDebug("NEGATIVE CACHE SET: {CacheKey}", key);

                return default;
            }

            // звичайний cache
            await _l2.SetAsync(key, value, expiration, group, cancellationToken);
            await _l1.SetAsync(key, value, l1Expiration, group, cancellationToken);

            _logger.LogDebug("CACHE POPULATED L1 + L2: {CacheKey}", key);

            return value;
        }
        finally
        {
            semaphore.Release();

            if (_locks.TryGetValue(key, out var current) && ReferenceEquals(current, semaphore))
                _locks.TryRemove(key, out _);
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, string? group = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var redisExpiration = expiration ?? TimeSpan.FromMinutes(_settings.DefaultExpirationMinutes);
        var l1Expiration = GetL1Expiration(redisExpiration);

        await _l2.SetAsync(key, value, redisExpiration, group, cancellationToken);
        await _l1.SetAsync(key, value, l1Expiration, group, cancellationToken);
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _l1.RemoveAsync(key, cancellationToken);
        await _l2.RemoveAsync(key, cancellationToken);
    }

    public async Task InvalidateGroupAsync(string group, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        await _l1.InvalidateGroupAsync(group, cancellationToken);
        await _l2.InvalidateGroupAsync(group, cancellationToken);
    }

    private TimeSpan GetL1Expiration(TimeSpan requestedExpiration)
    {
        var configuredL1Expiration = TimeSpan.FromMinutes(_settings.L1.ExpirationMinutes);
        return requestedExpiration <= configuredL1Expiration ? requestedExpiration : configuredL1Expiration;
    }
}