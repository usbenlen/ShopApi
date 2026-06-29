using Shop.Api.Interfaces;
using Shop.Domain.Models;

namespace Shop.Api.Services;

public class UserService : IUserService
{
    public User Register(User user)
    {
        return user;
    }
}
