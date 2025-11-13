namespace ZetaFin.Application.DTOs;

public class WhatsAppAuthRequestDto
{
    public string WhatsAppNumber { get; set; } = string.Empty;
}

public class WhatsAppAuthResponseDto
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string WhatsAppNumber { get; set; } = string.Empty;
}

public class LinkWhatsAppDto
{
    public Guid UserId { get; set; }
    public string WhatsAppNumber { get; set; } = string.Empty;
}