namespace ZetaFin.Application.DTOs;

public class RefreshTokenDto
{
    public required string RefreshToken { get; set; }
    public string? DeviceName { get; set; }
}
