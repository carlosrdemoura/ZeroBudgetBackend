namespace ZeroBudget.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public decimal Amount { get; private set; }   // positive = receivable (credit), negative = payable (debit)
    public DateOnly Date { get; private set; }
    public string? Description { get; private set; }
    public bool IsConsolidated { get; private set; }
    public double Position { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private Transaction() { }

    public static Transaction Create(decimal amount, DateOnly date, string? description, bool isConsolidated, double position, Guid? id = null)
    {
        if (amount == 0)
            throw new ArgumentException("Amount cannot be zero.", nameof(amount));
        if (double.IsNaN(position) || double.IsInfinity(position))
            throw new ArgumentException("Position must be a finite number.", nameof(position));
        if (id.HasValue && id.Value == Guid.Empty)
            throw new ArgumentException("Id cannot be an empty Guid.", nameof(id));

        return new Transaction
        {
            Id = id ?? Guid.NewGuid(),
            Amount = amount,
            Date = date,
            Description = description?.Trim(),
            IsConsolidated = isConsolidated,
            Position = position,
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

    public void SetPosition(double position)
    {
        if (double.IsNaN(position) || double.IsInfinity(position))
            throw new ArgumentException("Position must be a finite number.", nameof(position));

        Position = position;
    }
}
