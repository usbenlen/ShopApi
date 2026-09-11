namespace Shop.Api.Middlewares;

public class CancellationTestMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        await Task.Delay(TimeSpan.FromSeconds(5), context.RequestAborted);

        await next(context);
    }
}
