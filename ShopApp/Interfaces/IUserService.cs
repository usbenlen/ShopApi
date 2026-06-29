using Shop.Domain.Models;

namespace Shop.Api.Interfaces;

public interface IUserService
{
    User Register(User user);
}
