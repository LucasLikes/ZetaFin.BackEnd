namespace ZetaFin.Domain.Entities;

public class AuditLog
{
    public Guid Id { get; private set; }
    public Guid? UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string Resource { get; private set; } = string.Empty;
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;
    public string Status { get; private set; } = string.Empty; // Success, Failure
    public string? Details { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public AuditLog() { }

    public AuditLog(
        string action,
        string resource,
        string ipAddress,
        string userAgent,
        string status = "Success",
        Guid? userId = null,
        string? details = null)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Action = action;
        Resource = resource;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        Status = status;
        Details = details;
        CreatedAt = DateTime.UtcNow;
    }
}
