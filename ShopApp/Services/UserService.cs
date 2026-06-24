using ShopApp.Interfaces;
using ShopDomain.Models;

namespace ShopApp.Services;

public class UserService : IUserService
{
    public User Register(User user)
    {
        return user;
    }
}
