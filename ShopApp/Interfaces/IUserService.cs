using ShopDomain.Models;

namespace ShopApp.Interfaces;

public interface IUserService
{
    User Register(User user);
}
