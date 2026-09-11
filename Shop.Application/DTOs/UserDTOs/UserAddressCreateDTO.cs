using System.ComponentModel.DataAnnotations;

namespace Shop.Application.DTOs.UserDTOs;

public class UserAddressCreateDTO
{
    [Required]
    [MaxLength(100)]
    public string Country { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string City { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Street { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Building { get; set; } = string.Empty;

    [MaxLength(20)]
    public string? Apartment { get; set; }

    [MaxLength(20)]
    public string? PostalCode { get; set; }
}
