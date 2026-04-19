namespace ZeroBudget.Domain.Entities;

public class Transaction
{
    public Guid Id { get; private set; }
    public Guid AccountId { get; private set; }
    public Guid? CategoryId { get; private set; }
    public decimal Amount { get; private set; }   // positive = inflow, negative = outflow
    public DateOnly Date { get; private set; }
    public string? Memo { get; private set; }
    public bool AffectsBudget { get; private set; }
    public DateTime CreatedAt { get; private set; }

    public Category? Category { get; private set; }
    public Account? Account { get; private set; }

    private Transaction() { }

    public static Transaction Create(Guid accountId, decimal amount, DateOnly date, Guid? categoryId, string? memo = null, bool affectsBudget = true)
    {
        if (amount == 0)
            throw new ArgumentException("Amount cannot be zero.", nameof(amount));

        return new Transaction
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            CategoryId = categoryId,
            Amount = amount,
            Date = date,
            Memo = memo?.Trim(),
            AffectsBudget = affectsBudget,
            CreatedAt = DateTime.UtcNow
        };
    }

    public static Transaction CreateInitialBalance(Guid accountId, decimal amount, DateOnly date, Guid inflowCategoryId)
        => new()
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            CategoryId = inflowCategoryId,
            Amount = amount,
            Date = date,
            Memo = "Initial Balance",
            AffectsBudget = true,
            CreatedAt = DateTime.UtcNow
        };

    public static Transaction CreateAdjustment(Guid accountId, decimal amount, DateOnly date, Guid inflowCategoryId, string? memo = null)
        => new()
        {
            Id = Guid.NewGuid(),
            AccountId = accountId,
            CategoryId = inflowCategoryId,
            Amount = amount,
            Date = date,
            Memo = memo?.Trim() ?? "Balance Adjustment",
            AffectsBudget = true,
            CreatedAt = DateTime.UtcNow
        };

    public void Update(decimal amount, Guid? categoryId, DateOnly date, string? memo, bool affectsBudget)
    {
        if (amount == 0)
            throw new ArgumentException("Amount cannot be zero.", nameof(amount));

        Amount = amount;
        CategoryId = categoryId;
        Date = date;
        Memo = memo?.Trim();
        AffectsBudget = affectsBudget;
    }
}
