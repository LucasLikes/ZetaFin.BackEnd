namespace ZetaFin.Application.DTOs
{
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public DateTime? DueDate { get; set; }
    }

    public class CreateExpenseRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }

    public class UpdateExpenseRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime? DueDate { get; set; }
    }
}
