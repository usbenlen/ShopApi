using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.UserDTOs;

public class UserCreateDTO
{
    [Required]
    [MinLength(5)]
    [EmailAddress]
    public string Email { get; set; } = null!;

    [Required]
    [MinLength(5)]
    public string Password { get; set; } = null!;
}
