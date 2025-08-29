using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZetaFin.Domain.Entities;

public class Deposit
{
    private User? user;

    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime Date { get; private set; }
    public string Source { get; private set; } = string.Empty;
    public Guid GoalId { get; private set; }
    public Goal Goal { get; private set; }
    public Guid UserId { get; private set; }
    public User User { get; private set; }


    public Deposit(decimal amount, DateTime date, string source, Guid goalId, Guid userId)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive");
        if (string.IsNullOrWhiteSpace(source)) throw new ArgumentException("Source is required");
        if (goalId == Guid.Empty) throw new ArgumentException("GoalId is invalid");
        if (userId == Guid.Empty) throw new ArgumentException("UserId is invalid");

        Id = Guid.NewGuid();
        Amount = amount;
        Date = date;
        Source = source;
        GoalId = goalId;
        UserId = userId;
    }

    public Deposit(decimal amount, DateTime date, string source, Guid goalId, User? user)
    {
        Amount = amount;
        Date = date;
        Source = source;
        GoalId = goalId;
        this.user = user;
    }
}
