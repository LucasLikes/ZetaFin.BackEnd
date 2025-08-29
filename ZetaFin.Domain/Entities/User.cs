namespace ZetaFin.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;

    public ICollection<UserGoal> UserGoals { get; private set; } = new List<UserGoal>();

    public User(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required");
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email is required");

        Id = Guid.NewGuid();
        Name = name;
        Email = email;
    }
}
