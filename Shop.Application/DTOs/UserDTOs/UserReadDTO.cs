using Shop.Domain.Enums;

namespace Shop.Application.DTOs.UserDTOs;

public class UserReadDTO
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
