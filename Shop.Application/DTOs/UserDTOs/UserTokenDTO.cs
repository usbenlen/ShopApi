using Shop.Domain.Enums;

namespace Shop.Application.DTOs.UserDTOs;

public sealed record UserTokenDTO
{
    public Guid Id { get; init; }

    public string Email { get; init; } = string.Empty;

    public UserRole Role { get; init; }
}