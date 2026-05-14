namespace ZeroBudget.Application.DTOs;

public class TransactionDTO
{
    public TransactionDTO(
        Guid id,
        decimal amount,
        DateOnly date,
        string? description,
        bool isConsolidated,
        double position,
        DateTime createdAt)
    {
        Id = id;
        Amount = amount;
        Date = date;
        Description = description;
        IsConsolidated = isConsolidated;
        Position = position;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public decimal Amount { get; }
    public DateOnly Date { get; }
    public string? Description { get; }
    public bool IsConsolidated { get; }
    public double Position { get; }
    public DateTime CreatedAt { get; }
}
