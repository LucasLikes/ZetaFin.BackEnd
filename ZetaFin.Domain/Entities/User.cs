using System.Net.Mail;

namespace ZetaFin.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public string Role { get; private set; } = "User";
    public bool IsEmailConfirmed { get; private set; } = false;
    public bool IsActive { get; private set; } = true;
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }
    public int FailedLoginAttempts { get; private set; } = 0;
    public DateTime? LockedUntil { get; private set; }

    public ICollection<UserGoal> UserGoals { get; private set; } = new List<UserGoal>();
    public ICollection<UserWhatsApp> UserWhatsApps { get; private set; } = new List<UserWhatsApp>();
    public ICollection<RefreshToken> RefreshTokens { get; private set; } = new List<RefreshToken>();
    public ICollection<UserSession> Sessions { get; private set; } = new List<UserSession>();

    public User() { }

    public User(string name, string email, string password, string role = "User")
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required");

        try
        {
            _ = new MailAddress(email);
        }
        catch (FormatException)
        {
            throw new ArgumentException("Email is not in a valid format.");
        }

        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        Role = role;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }

    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    public void ConfirmEmail()
    {
        IsEmailConfirmed = true;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash is required");
        PasswordHash = newPasswordHash;
    }

    public void RecordSuccessfulLogin()
    {
        LastLoginAt = DateTime.UtcNow;
        FailedLoginAttempts = 0;
        LockedUntil = null;
    }

    public void RecordFailedLogin()
    {
        FailedLoginAttempts++;
        if (FailedLoginAttempts >= 5)
        {
            LockedUntil = DateTime.UtcNow.AddMinutes(30);
        }
    }

    public bool IsLockedOut()
    {
        return LockedUntil.HasValue && DateTime.UtcNow < LockedUntil;
    }

    public void UnlockAccount()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
