namespace ShopApp.Middleware;

public class RequestTimerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestTimerMiddleware> _logger;

    public RequestTimerMiddleware(RequestDelegate next, ILogger<RequestTimerMiddleware> logger)
    {
        _next = next; // Посилання на наступний middleware у черзі
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var watch = System.Diagnostics.Stopwatch.StartNew();

        // 1. Код ДО наступного компонента
        _logger.LogInformation("Початок запиту: {Path}", context.Request.Path);

        // 2. Передаємо керування далі
        await _next(context);

        // 3. Код ПІСЛЯ того, як відпрацював контролер
        watch.Stop();
        _logger.LogInformation("Запит завершено за {Ms} мс", watch.ElapsedMilliseconds);



        context.Response.StatusCode = 404;
        context.Response.ContentType = "application/json";

        var response = new
        {
            message = "Error"
        };

        await context.Response.WriteAsJsonAsync(response);
        return;
    }
}
