namespace ZetaFin.Application.DTOs;

public class ChangePasswordDto
{
    public required string CurrentPassword { get; set; }
    public required string NewPassword { get; set; }
    public required string NewPasswordConfirmation { get; set; }
}
