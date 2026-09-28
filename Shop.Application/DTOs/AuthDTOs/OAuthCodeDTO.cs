namespace Shop.Application.DTOs.AuthDTOs;

public sealed record OAuthCodeDTO
{
    public string Code { get; init; } = string.Empty;
}
