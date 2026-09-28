namespace Shop.Application.DTOs.AuthDTOs;

public class OAuthLoginResultDTO
{
    public string AccessToken { get; set; } = string.Empty;
    public RefreshTokenDTO RefreshToken { get; set; } = null!;
}