namespace Shop.Application.Interfaces.Cache;

public interface ICacheStore
{
    Task<CacheLookupResult<T>> GetAsync<T>(string key, CancellationToken cancellationToken = default);
    Task SetAsync<T>(string key, T value, TimeSpan expiration, string? group = null, CancellationToken cancellationToken = default);
    Task SetNegativeAsync(string key, TimeSpan expiration, string? group = null, CancellationToken cancellationToken = default);
    Task RemoveAsync(string key, CancellationToken cancellationToken = default);
    Task InvalidateGroupAsync(string group, CancellationToken cancellationToken = default);
}