using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.UserDTOs;

/// <summary>
/// DTO для аутентифікації
/// </summary>
public class UserLoginDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
