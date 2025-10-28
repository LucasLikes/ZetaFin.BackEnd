using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // Construtor sem parâmetros para o EF no tempo de design
    public ApplicationDbContext() { }

    public DbSet<Goal> Goals { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Definindo a chave primária para a entidade User
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);  // Especifica que 'Id' é a chave primária da tabela

        // Definindo as propriedades como obrigatórias (campo NOT NULL no banco de dados)
        modelBuilder.Entity<User>()
            .Property(u => u.Name)
            .IsRequired();  // 'Name' é obrigatório

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();  // 'Email' é obrigatório

        modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .IsRequired();  // 'PasswordHash' é obrigatório

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .IsRequired();  // 'Role' é obrigatório, com valor padrão 'User'

        // Configurando o relacionamento com a entidade UserGoal
        modelBuilder.Entity<User>()
            .HasMany(u => u.UserGoals)  // Um usuário pode ter muitos UserGoals
            .WithOne()  // Define o relacionamento um-para-muitos
            .HasForeignKey(ug => ug.UserId);  // Especifica que o 'UserId' é a chave estrangeira em UserGoal

        modelBuilder.Entity<UserGoal>()
        .HasKey(ug => new { ug.UserId, ug.GoalId });  // Definindo a chave composta

        // Outras configurações do modelo
        modelBuilder.Entity<UserGoal>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.UserGoals)
            .HasForeignKey(ug => ug.UserId);

        modelBuilder.Entity<UserGoal>()
            .HasOne(ug => ug.Goal)
            .WithMany()  // Caso o Goal não tenha uma coleção de UserGoals, use WithMany()
            .HasForeignKey(ug => ug.GoalId);
    }
}
