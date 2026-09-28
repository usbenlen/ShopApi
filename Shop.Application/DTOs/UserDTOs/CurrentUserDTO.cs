namespace Shop.Application.DTOs.UserDTOs;

public sealed record CurrentUserDTO
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
}