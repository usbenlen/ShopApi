using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.UserDTOs;

public class UpdateUserStatusDTO
{
    [Required]
    public bool IsActive { get; set; }
}