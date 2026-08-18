using System.ComponentModel.DataAnnotations;
using Shop.Domain.Enums;

namespace Shop.Application.DTOs.UserDTOs;

public class CreateStaffUserDTO
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public UserRole Role { get; set; }
}