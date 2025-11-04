using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Persistence;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    // Construtor sem parâmetros (para EF no tempo de design)
    public ApplicationDbContext() { }

    // DbSets existentes
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configurações que você já tinha...
        modelBuilder.Entity<User>()
            .HasKey(u => u.Id);

        modelBuilder.Entity<User>()
            .Property(u => u.Name)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Email)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .IsRequired();

        modelBuilder.Entity<User>()
            .Property(u => u.Role)
            .IsRequired();

        modelBuilder.Entity<User>()
            .HasMany(u => u.UserGoals)
            .WithOne()
            .HasForeignKey(ug => ug.UserId);

        modelBuilder.Entity<UserGoal>()
            .HasKey(ug => new { ug.UserId, ug.GoalId });

        modelBuilder.Entity<UserGoal>()
            .HasOne(ug => ug.User)
            .WithMany(u => u.UserGoals)
            .HasForeignKey(ug => ug.UserId);

        modelBuilder.Entity<UserGoal>()
            .HasOne(ug => ug.Goal)
            .WithMany()
            .HasForeignKey(ug => ug.GoalId);

        modelBuilder.Entity<Expense>()
            .Property(e => e.Name)
            .IsRequired();

        modelBuilder.Entity<Expense>()
            .Property(e => e.Value)
            .HasColumnType("decimal(18,2)");
    }
}