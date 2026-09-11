namespace Shop.Api.Middlewares;

public class CancellationTokenHandleMiddleware(RequestDelegate _next, ILogger<CancellationTokenHandleMiddleware> _logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex) when (ex is OperationCanceledException or TaskCanceledException)
        {
            _logger.LogError("Request is canceled");
        }
    }
}
