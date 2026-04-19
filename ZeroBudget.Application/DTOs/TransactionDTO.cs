namespace ZeroBudget.Application.DTOs;

public class TransactionDTO
{
    public TransactionDTO(
        Guid id,
        Guid accountId,
        Guid? categoryId,
        string? categoryName,
        decimal amount,
        DateOnly date,
        string? memo,
        bool affectsBudget,
        DateTime createdAt)
    {
        Id = id;
        AccountId = accountId;
        CategoryId = categoryId;
        CategoryName = categoryName;
        Amount = amount;
        Date = date;
        Memo = memo;
        AffectsBudget = affectsBudget;
        CreatedAt = createdAt;
    }

    public Guid Id { get; }
    public Guid AccountId { get; }
    public Guid? CategoryId { get; }
    public string? CategoryName { get; }
    public decimal Amount { get; }
    public DateOnly Date { get; }
    public string? Memo { get; }
    public bool AffectsBudget { get; }
    public DateTime CreatedAt { get; }
}
