using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public ApplicationDbContext() { }

    // DbSets existentes
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }
    public DbSet<Expense> Expenses { get; set; }

    // Novos DbSets
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Receipt> Receipts { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =================== CONFIGURAÇÕES EXISTENTES ===================

        // User
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

        // UserGoal
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

        // Expense
        modelBuilder.Entity<Expense>()
            .Property(e => e.Name)
            .IsRequired();

        modelBuilder.Entity<Expense>()
            .Property(e => e.Value)
            .HasColumnType("decimal(18,2)");

        // =================== NOVAS CONFIGURAÇÕES - TRANSACTION ===================

        modelBuilder.Entity<Transaction>()
            .HasKey(t => t.Id);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Value)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Description)
            .HasMaxLength(500)
            .IsRequired();

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Category)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Type)
            .HasConversion<string>()
            .IsRequired();

        modelBuilder.Entity<Transaction>()
            .Property(t => t.ExpenseType)
            .HasConversion<string>();

        modelBuilder.Entity<Transaction>()
            .Property(t => t.ReceiptOcrData)
            .HasColumnType("TEXT");

        // Índices para Transaction
        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.UserId);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => t.Date);

        modelBuilder.Entity<Transaction>()
            .HasIndex(t => new { t.UserId, t.Date });

        // Relacionamento Transaction -> Receipt (1:1 opcional)
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Receipt)
            .WithOne(r => r.Transaction)
            .HasForeignKey<Receipt>(r => r.TransactionId)
            .OnDelete(DeleteBehavior.SetNull)
            .IsRequired(false);

        // =================== NOVAS CONFIGURAÇÕES - RECEIPT ===================

        modelBuilder.Entity<Receipt>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<Receipt>()
            .Property(r => r.FileName)
            .HasMaxLength(255)
            .IsRequired();

        modelBuilder.Entity<Receipt>()
            .Property(r => r.FileUrl)
            .HasMaxLength(500)
            .IsRequired();

        modelBuilder.Entity<Receipt>()
            .Property(r => r.MimeType)
            .HasMaxLength(100)
            .IsRequired();

        modelBuilder.Entity<Receipt>()
            .Property(r => r.OcrDataJson)
            .HasColumnType("TEXT");

        // Índices para Receipt
        modelBuilder.Entity<Receipt>()
            .HasIndex(r => r.UserId);

        modelBuilder.Entity<Receipt>()
            .HasIndex(r => r.TransactionId)
            .IsUnique()
            .HasFilter("[TransactionId] IS NOT NULL");

        // Relacionamento Receipt -> User
        modelBuilder.Entity<Receipt>()
            .HasOne(r => r.User)
            .WithMany()
            .HasForeignKey(r => r.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}