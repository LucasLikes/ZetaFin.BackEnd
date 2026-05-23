namespace ZetaFin.Application.Interfaces;

public interface IPasswordService
{
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    (bool IsValid, List<string> Errors) ValidatePasswordStrength(string password);
}
