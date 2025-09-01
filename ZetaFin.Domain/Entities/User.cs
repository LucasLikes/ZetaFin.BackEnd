using BCrypt.Net;
using System.Net.Mail;


namespace ZetaFin.Domain.Entities;
public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    // Adicionar um campo para roles
    public string Role { get; private set; } = "User";  // Definir um role padrão como "User"

    public ICollection<UserGoal> UserGoals { get; private set; } = new List<UserGoal>();

    public bool IsEmailConfirmed { get; private set; } = false;

    public User(string name, string email, string password, string role = "User")
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required");
        if (string.IsNullOrWhiteSpace(password)) throw new ArgumentException("Password is required");

        // Validar o formato do email
        try
        {
            var mailAddress = new MailAddress(email);
        }
        catch (FormatException)
        {
            throw new ArgumentException("Email is not in a valid format.");
        }

        Id = Guid.NewGuid();
        Name = name;
        Email = email;
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        Role = role; // Pode ser 'User', 'Admin', etc.
    }

    // Verificar se a senha fornecida corresponde ao hash armazenado
    public bool VerifyPassword(string password)
    {
        return BCrypt.Net.BCrypt.Verify(password, PasswordHash);
    }

    // Confirmar e-mail
    public void ConfirmEmail()
    {
        IsEmailConfirmed = true;
    }
}
