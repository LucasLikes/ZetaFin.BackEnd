using Microsoft.EntityFrameworkCore;
using ZetaFin.Domain.Entities;

namespace ZetaFin.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public ApplicationDbContext() { }

    // DbSets
    public DbSet<Goal> Goals { get; set; }
    public DbSet<Deposit> Deposits { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserGoal> UserGoals { get; set; }
    public DbSet<Expense> Expenses { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Receipt> Receipts { get; set; }
    public DbSet<UserWhatsApp> UserWhatsApps { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // =================== USER ===================
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(u => u.IsEmailConfirmed)
                .IsRequired();

            entity.Property(u => u.IsActive)
                .IsRequired();

            entity.Property(u => u.CreatedAt)
                .IsRequired();

            entity.Property(u => u.FailedLoginAttempts)
                .IsRequired();

            entity.HasIndex(u => u.CreatedAt);
        });

        // =================== GOAL ===================
        modelBuilder.Entity<Goal>(entity =>
        {
            entity.HasKey(g => g.Id);

            entity.Property(g => g.Description)
                .IsRequired()
                .HasMaxLength(500);

            entity.Property(g => g.TargetAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(g => g.CurrentAmount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(g => g.CreatedAt)
                .IsRequired();

            entity.HasIndex(g => g.CreatedAt);
        });

        // =================== DEPOSIT ===================
        modelBuilder.Entity<Deposit>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Amount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(d => d.Source)
                .IsRequired()
                .HasMaxLength(200);

            entity.HasOne(d => d.Goal)
                .WithMany(g => g.Deposits)
                .HasForeignKey(d => d.GoalId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User)
                .WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(d => d.Date);
        });

        // =================== USER GOAL ===================
        modelBuilder.Entity<UserGoal>(entity =>
        {
            entity.HasKey(ug => new { ug.UserId, ug.GoalId });

            entity.Property(ug => ug.CustomMonthlyTarget)
                .HasColumnType("decimal(18,2)");

            entity.HasOne(ug => ug.User)
                .WithMany(u => u.UserGoals)
                .HasForeignKey(ug => ug.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(ug => ug.Goal)
                .WithMany()
                .HasForeignKey(ug => ug.GoalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =================== EXPENSE ===================
        modelBuilder.Entity<Expense>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Value)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(e => e.Category)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(e => e.UserId);
            entity.HasIndex(e => e.Date);
        });

        // =================== TRANSACTION ===================
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasKey(t => t.Id);

            entity.Property(t => t.Value)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            entity.Property(t => t.Description)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(t => t.Category)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(t => t.Type)
                .HasConversion<string>()
                .IsRequired();

            entity.Property(t => t.ExpenseType)
                .HasConversion<string>();

            entity.HasIndex(t => t.UserId);
            entity.HasIndex(t => t.Date);
            entity.HasIndex(t => new { t.UserId, t.Date });
            entity.HasIndex(t => t.Type);

            // Relacionamento 1:1 com Receipt
            entity.HasOne(t => t.Receipt)
                .WithOne(r => r.Transaction)
                .HasForeignKey<Receipt>(r => r.TransactionId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);
        });

        // =================== RECEIPT ===================
        modelBuilder.Entity<Receipt>(entity =>
        {
            entity.HasKey(r => r.Id);

            entity.Property(r => r.FileName)
                .HasMaxLength(255)
                .IsRequired();

            entity.Property(r => r.FileUrl)
                .HasMaxLength(500)
                .IsRequired();

            entity.Property(r => r.MimeType)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(r => r.OcrDataJson)
                .HasColumnType("jsonb"); // PostgreSQL JSONB para melhor performance

            entity.HasIndex(r => r.UserId);
            entity.HasIndex(r => r.TransactionId)
                .IsUnique()
                .HasFilter("\"TransactionId\" IS NOT NULL");

            entity.HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =================== USER WHATSAPP ===================
        modelBuilder.Entity<UserWhatsApp>(entity =>
        {
            entity.HasKey(w => w.Id);

            entity.Property(w => w.WhatsAppNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(w => w.IsActive)
                .IsRequired();

            entity.Property(w => w.CreatedAt)
                .IsRequired();

            entity.HasIndex(w => w.UserId);
            entity.HasIndex(w => w.WhatsAppNumber).IsUnique();

            entity.HasOne(w => w.User)
                .WithMany(u => u.UserWhatsApps)
                .HasForeignKey(w => w.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =================== REFRESH TOKEN ===================
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(rt => rt.Id);

            entity.Property(rt => rt.Token)
                .IsRequired()
                .HasMaxLength(1000);

            entity.Property(rt => rt.DeviceName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(rt => rt.DeviceType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(rt => rt.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(rt => rt.CreatedAt)
                .IsRequired();

            entity.Property(rt => rt.ExpiresAt)
                .IsRequired();

            entity.Property(rt => rt.RevokeReason)
                .HasMaxLength(500);

            entity.HasIndex(rt => rt.Token)
                .IsUnique();
            entity.HasIndex(rt => rt.UserId);
            entity.HasIndex(rt => new { rt.UserId, rt.RevokedAt });

            entity.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =================== USER SESSION ===================
        modelBuilder.Entity<UserSession>(entity =>
        {
            entity.HasKey(us => us.Id);

            entity.Property(us => us.DeviceName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(us => us.DeviceType)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(us => us.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(us => us.UserAgent)
                .HasMaxLength(1000);

            entity.Property(us => us.CreatedAt)
                .IsRequired();

            entity.Property(us => us.LastAccessAt)
                .IsRequired();

            entity.Property(us => us.IsActive)
                .IsRequired();

            entity.HasIndex(us => us.UserId);
            entity.HasIndex(us => new { us.UserId, us.IsActive });

            entity.HasOne(us => us.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // =================== AUDIT LOG ===================
        modelBuilder.Entity<AuditLog>(entity =>
        {
            entity.HasKey(al => al.Id);

            entity.Property(al => al.Action)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(al => al.Resource)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(al => al.IpAddress)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(al => al.UserAgent)
                .HasMaxLength(1000);

            entity.Property(al => al.Status)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(al => al.Details)
                .HasColumnType("jsonb");

            entity.Property(al => al.CreatedAt)
                .IsRequired();

            entity.HasIndex(al => al.UserId);
            entity.HasIndex(al => al.Action);
            entity.HasIndex(al => al.CreatedAt);
            entity.HasIndex(al => new { al.UserId, al.CreatedAt });
        });
    }
}