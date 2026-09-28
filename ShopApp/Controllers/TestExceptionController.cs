using Microsoft.AspNetCore.Mvc;
using Shop.Api.Exceptions;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/test-exceptions")]
public class TestExceptionController : ControllerBase
{
    [HttpGet("not-found")]
    public IActionResult TestNotFound()
    {
        throw new NotFoundException("Тест: ресурс не знайдено");
    }

    [HttpGet("validation")]
    public IActionResult TestValidation()
    {
        throw new ValidationAppException(
            new Dictionary<string, string[]>
            {
                ["Email"] = ["Email має бути вказаний."],
                ["Password"] = ["Пароль має містити щонайменше 8 символів."]
            });
    }

    [HttpGet("server-error")]
    public IActionResult TestServerError()
    {
        throw new Exception("Тестова непередбачена помилка.");
    }
}
