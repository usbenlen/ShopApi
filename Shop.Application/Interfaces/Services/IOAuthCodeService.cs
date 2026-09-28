namespace Shop.Application.Interfaces.Services;

public interface IOAuthCodeService
{
    Task<string> CreateAsync(string accessToken, CancellationToken cancellationToken = default);
    Task<string?> ConsumeAsync(string code, CancellationToken cancellationToken = default);
}
