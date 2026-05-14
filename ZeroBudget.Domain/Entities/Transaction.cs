namespace ZeroBudget.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }   // positive = receivable (credit), negative = payable (debit)
    public DateOnly Date { get; private set; }
    public string? Description { get; private set; }
    public bool IsConsolidated { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() { }

    public static Transaction Create(decimal amount, DateOnly date, string? description, bool isConsolidated)
    {
        if (amount == 0)
            throw new ArgumentException("Amount cannot be zero.", nameof(amount));

        return new Transaction
        {
            Id = Guid.NewGuid(),
            Amount = amount,
            Date = date,
            Description = description?.Trim(),
            IsConsolidated = isConsolidated,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(decimal amount, DateOnly date, string? description, bool isConsolidated)
    {
        if (amount == 0)
            throw new ArgumentException("Amount cannot be zero.", nameof(amount));

        Amount = amount;
        Date = date;
        Description = description?.Trim();
        IsConsolidated = isConsolidated;
    }

    public void SetConsolidated(bool isConsolidated)
    {
        IsConsolidated = isConsolidated;
    }
}
