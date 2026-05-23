namespace ZetaFin.Application.DTOs;

public class UserSessionDto
{
    public Guid Id { get; set; }
    public string DeviceName { get; set; } = string.Empty;
    public string DeviceType { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime LastAccessAt { get; set; }
    public bool IsActive { get; set; }
    public bool IsCurrentSession { get; set; }
}
