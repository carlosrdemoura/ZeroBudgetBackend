namespace ZeroBudget.Application.DTOs;

public class TransactionDTO
{
    public TransactionDTO(
        Guid id,
        decimal amount,
        DateOnly date,
        string? description,
        bool isConsolidated,
        DateTime createdAt)
    {
        Id = id;
        Amount = amount;
        Date = date;
        Description = description;
        IsConsolidated = isConsolidated;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public decimal Amount { get; }
    public DateOnly Date { get; }
    public string? Description { get; }
    public bool IsConsolidated { get; }
    public DateTime CreatedAt { get; }
}
