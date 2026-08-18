using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.AuthDTOs;

public class ResetPasswordDTO
{
    [Required]
    public string Token { get; set; } = string.Empty;

    [Required]
    [MinLength(8)]
    public string NewPassword { get; set; } = string.Empty;
}