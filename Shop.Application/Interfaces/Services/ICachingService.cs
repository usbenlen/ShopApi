namespace Shop.Application.Interfaces.Services;

public interface ICachingService
{
    Task<T?> GetAsync<T>(string key);
    Task<T?> GetOrCreateAsync<T>(string key, Func<Task<T?>> factory, TimeSpan expiration, string? group = null);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, string? group = null);
    Task RemoveAsync(string key);
    Task InvalidateGroupAsync(string group);
}
