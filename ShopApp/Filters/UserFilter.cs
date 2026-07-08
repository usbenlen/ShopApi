//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Shop.Domain.Models;

//namespace Shop.Api.Filters;

//public class UserFilter : ActionFilterAttribute
//{
//    public override void OnActionExecuting(ActionExecutingContext context)
//    {
//        var user = context.ActionArguments["user"] as User;

//        if (user != null && user.Id == 1 && user.Login == "admin") return;

//        context.Result = new JsonResult(new { message = "No authorization" })
//        {
//            StatusCode = 401
//        };
//    }
//}