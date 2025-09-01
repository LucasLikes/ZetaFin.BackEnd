using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Goal> Goals { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Definindo a chave composta para a entidade UserGoal
        modelBuilder.Entity<UserGoal>()
            .HasKey(ug => new { ug.UserId, ug.GoalId });

        // Qualquer outra configuração específica do modelo
    }
}
