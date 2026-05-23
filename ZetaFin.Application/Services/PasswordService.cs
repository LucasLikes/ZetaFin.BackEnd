using BCrypt.Net;
using ZetaFin.Application.Interfaces;

namespace ZetaFin.Application.Services;

public class PasswordService : IPasswordService
{
    private const int MinPasswordLength = 8;
    private const int BCryptWorkFactor = 12;

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, BCryptWorkFactor);
    }

    public bool VerifyPassword(string password, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            return false;
        }
    }

    public (bool IsValid, List<string> Errors) ValidatePasswordStrength(string password)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(password))
        {
            errors.Add("Password is required");
            return (false, errors);
        }

        if (password.Length < MinPasswordLength)
        {
            errors.Add($"Password must be at least {MinPasswordLength} characters long");
        }

        if (!password.Any(c => char.IsUpper(c)))
        {
            errors.Add("Password must contain at least one uppercase letter");
        }

        if (!password.Any(c => char.IsLower(c)))
        {
            errors.Add("Password must contain at least one lowercase letter");
        }

        if (!password.Any(char.IsDigit))
        {
            errors.Add("Password must contain at least one digit");
        }

        if (!password.Any(c => !char.IsLetterOrDigit(c)))
        {
            errors.Add("Password must contain at least one special character");
        }

        return (errors.Count == 0, errors);
    }
}
