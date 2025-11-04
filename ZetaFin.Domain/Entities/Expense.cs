using System;
using ZetaFin.Domain.Common;

namespace ZetaFin.Domain.Entities
{
    public class Expense : BaseEntity
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public DateTime? DueDate { get; set; }

        // Regras básicas de domínio
        public bool IsOverdue => DueDate.HasValue && DueDate.Value < DateTime.UtcNow;
    }
}
