using System.ComponentModel.DataAnnotations;
using Shop.Domain.Enums;

namespace Shop.Application.DTOs.UserDTOs;

public class UpdateUserRoleDTO
{
    [Required]
    public UserRole Role { get; set; }
}