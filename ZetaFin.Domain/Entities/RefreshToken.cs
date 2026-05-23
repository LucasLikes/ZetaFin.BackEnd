namespace ZetaFin.Domain.Entities;

public class RefreshToken
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string Token { get; private set; } = string.Empty;
    public string DeviceName { get; private set; } = string.Empty;
    public string DeviceType { get; private set; } = string.Empty; // Web, Mobile, Desktop
    public string IpAddress { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string? RevokeReason { get; private set; }
    public User User { get; private set; } = null!;

    public RefreshToken() { }

    public RefreshToken(
        Guid userId,
        string token,
        string deviceName,
        string deviceType,
        string ipAddress,
        int expirationDays = 30)
    {
        if (string.IsNullOrWhiteSpace(token))
            throw new ArgumentException("Token is required", nameof(token));
        if (string.IsNullOrWhiteSpace(deviceName))
            throw new ArgumentException("Device name is required", nameof(deviceName));
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address is required", nameof(ipAddress));

        Id = Guid.NewGuid();
        UserId = userId;
        Token = token;
        DeviceName = deviceName;
        DeviceType = deviceType;
        IpAddress = ipAddress;
        CreatedAt = DateTime.UtcNow;
        ExpiresAt = DateTime.UtcNow.AddDays(expirationDays);
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsRevoked => RevokedAt.HasValue;
    public bool IsActive => !IsExpired && !IsRevoked;

    public void Revoke(string reason = "Manual revocation")
    {
        RevokedAt = DateTime.UtcNow;
        RevokeReason = reason;
    }

    public void RevokeByExpiration()
    {
        RevokedAt = DateTime.UtcNow;
        RevokeReason = "Expired";
    }
}
