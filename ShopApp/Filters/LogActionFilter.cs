using Microsoft.AspNetCore.Mvc.Filters;

namespace Shop.Api.Filters;

public class LogActionFilter : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext context)
    {
        Console.WriteLine("Before running function");
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        Console.WriteLine("After running function");
    }
}
