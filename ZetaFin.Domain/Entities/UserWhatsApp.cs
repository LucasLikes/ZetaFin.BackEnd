using System;

namespace ZetaFin.Domain.Entities;

public class UserWhatsApp
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string WhatsAppNumber { get; private set; } = string.Empty; // formato: +5511999999999
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? LastMessageAt { get; private set; }

    // Navigation
    public User User { get; private set; } = null!;

    // EF Constructor
    private UserWhatsApp() { }

    public UserWhatsApp(Guid userId, string whatsAppNumber)
    {
        if (string.IsNullOrWhiteSpace(whatsAppNumber))
            throw new ArgumentException("WhatsApp number is required");

        // Validar formato: +5511999999999
        if (!whatsAppNumber.StartsWith("+") || whatsAppNumber.Length < 12)
            throw new ArgumentException("Invalid WhatsApp number format. Use +5511999999999");

        Id = Guid.NewGuid();
        UserId = userId;
        WhatsAppNumber = whatsAppNumber.Trim();
        IsActive = true;
        CreatedAt = DateTime.UtcNow;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;

    public void UpdateLastMessage()
    {
        LastMessageAt = DateTime.UtcNow;
    }
}