namespace ZetaFin.Application.DTOs;

public class MercadoPagoPixNotification
{
    public Guid GoalId { get; set; }
    public decimal Amount { get; set; }
    public string PixKey { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
}
