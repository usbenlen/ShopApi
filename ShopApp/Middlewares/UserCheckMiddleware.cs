//using System.Text.Json;
//using Shop.Domain.Models;

//namespace Shop.Api.Middleware;

//public class UserMiddlewareCheck
//{
//    private readonly RequestDelegate _next;
//    private readonly ILogger<UserMiddlewareCheck> _logger;

//    public UserMiddlewareCheck(RequestDelegate next, ILogger<UserMiddlewareCheck> logger)
//    {
//        _next = next;
//        _logger = logger;
//    }

//    public async Task InvokeAsync(HttpContext context)
//    {
//        if (context.Request.Method != "POST" || !context.Request.Path.ToString().ToLower().Equals("/api/user/register"))
//        {
//            await _next(context);
//        }

//        context.Request.EnableBuffering();
//        var user = await JsonSerializer.DeserializeAsync<User>(context.Request.Body);
//        context.Request.Body.Seek(0, SeekOrigin.Begin);

//        _logger.LogInformation($"User request received: {user?.Id} | {user?.Login}");

//        if (user != null && user.Id == 1 && user.Login == "admin")
//        {
//            _logger.LogInformation($"User authorized: {user.Id} | {user.Login}");

//            await _next(context);
//        }
//        else
//        {
//            _logger.LogWarning($"Unauthorized access attempt: {user?.Id} | {user?.Login}");
//            context.Response.StatusCode = 401;
//            await context.Response.WriteAsJsonAsync(new { message = "No authorization" });
//        }
//    }
//}