using Shop.Application.Interfaces.Helpers;

namespace Shop.Infrastructure.Helpers;

public class HashHelper : IHashHelper
{
    public string Hash(string password)
    {
        return BCrypt.Net.BCrypt.EnhancedHashPassword(password);
    }

    public bool IsPasswordValid(string password, string hash)
    {
        return BCrypt.Net.BCrypt.EnhancedVerify(password, hash);
    }
}
