namespace ZetaFin.Application.DTOs;

public class ResetPasswordDto
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required string NewPassword { get; set; }
    public required string NewPasswordConfirmation { get; set; }
}
