using Shop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shop.Application.DTOs.UserDTOs;

public class UserUpdateDTO
{
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;

}
