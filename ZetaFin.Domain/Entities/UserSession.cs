namespace ZetaFin.Domain.Entities;

public class UserSession
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string DeviceName { get; private set; } = string.Empty;
    public string DeviceType { get; private set; } = string.Empty; // Web, Mobile, Desktop
    public string IpAddress { get; private set; } = string.Empty;
    public string UserAgent { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }
    public DateTime LastAccessAt { get; private set; }
    public DateTime? TerminatedAt { get; private set; }
    public bool IsActive { get; private set; }
    public Guid? RefreshTokenId { get; private set; }
    public User User { get; private set; } = null!;

    public UserSession() { }

    public UserSession(
        Guid userId,
        string deviceName,
        string deviceType,
        string ipAddress,
        string userAgent)
    {
        if (string.IsNullOrWhiteSpace(deviceName))
            throw new ArgumentException("Device name is required", nameof(deviceName));
        if (string.IsNullOrWhiteSpace(ipAddress))
            throw new ArgumentException("IP address is required", nameof(ipAddress));

        Id = Guid.NewGuid();
        UserId = userId;
        DeviceName = deviceName;
        DeviceType = deviceType;
        IpAddress = ipAddress;
        UserAgent = userAgent;
        CreatedAt = DateTime.UtcNow;
        LastAccessAt = DateTime.UtcNow;
        IsActive = true;
    }

    public void UpdateLastAccess()
    {
        LastAccessAt = DateTime.UtcNow;
    }

    public void Terminate()
    {
        IsActive = false;
        TerminatedAt = DateTime.UtcNow;
    }

    public void SetRefreshTokenId(Guid tokenId)
    {
        RefreshTokenId = tokenId;
    }
}
