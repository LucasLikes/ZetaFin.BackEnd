using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZetaFin.Domain.Entities;

public class Deposit
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Source { get; private set; } = string.Empty;

    public Guid GoalId { get; private set; }
    public Goal Goal { get; private set; }

    public Deposit(decimal amount, DateTime date, string source, Guid goalId)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");

        Id = Guid.NewGuid();
        Amount = amount;
        Date = date;
        Source = source;
        GoalId = goalId;
    }
}
