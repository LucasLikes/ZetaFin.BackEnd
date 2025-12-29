using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZetaFin.Domain.Entities;
public class Goal
{
    public Guid Id { get; private set; }
    public string Description { get; private set; } = string.Empty;
    public decimal TargetAmount { get; private set; }
    public decimal CurrentAmount { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? TargetDate { get; private set; } = DateTime.UtcNow;
    public ICollection<UserGoal> UserGoals { get; private set; } = new List<UserGoal>();

    public int RemainingMonths
        => TargetDate.HasValue
            ? Math.Max(1, ((TargetDate.Value.Year - CreatedAt.Year) * 12) + TargetDate.Value.Month - CreatedAt.Month)
            : 1;

    public Goal(string description, decimal targetAmount, DateTime? targetDate = null)
    {
        Id = Guid.NewGuid();
        Description = description;
        TargetAmount = targetAmount;
        CurrentAmount = 0;
        CreatedAt = DateTime.UtcNow;
        TargetDate = targetDate;

        Validate();
    }

    private readonly List<Deposit> _deposits = new();
    public IReadOnlyCollection<Deposit> Deposits => _deposits.AsReadOnly();

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Description))
            throw new ArgumentException("Description cannot be empty");

        if (TargetAmount <= 0)
            throw new ArgumentException("Target amount must be greater than zero");
    }

    public void AddAmount(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount must be positive");

        CurrentAmount += amount;
    }

    public void AddDeposit(Deposit deposit)
    {
        if (deposit == null) throw new ArgumentNullException(nameof(deposit));
        _deposits.Add(deposit);
        AddAmount(deposit.Amount);
    }

    public decimal GetRequiredMonthlyContribution()
    {
        return Math.Round(TargetAmount / RemainingMonths, 2);
    }
}

