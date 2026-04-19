using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Domain.Entities;

public class BudgetEntry
{
    public Guid Id { get; private set; }
    public Guid CategoryId { get; private set; }
    public YearMonth Month { get; private set; } = null!;
    public decimal Assigned { get; private set; }

    public Category Category { get; private set; } = null!;

    // EF Core constructor
    private BudgetEntry() { }

    public static BudgetEntry Create(Guid categoryId, YearMonth month, decimal assigned = 0)
    {
        if (assigned < 0)
            throw new ArgumentOutOfRangeException(nameof(assigned), "Assigned amount cannot be negative.");

        return new BudgetEntry
        {
            Id = Guid.NewGuid(),
            CategoryId = categoryId,
            Month = month,
            Assigned = assigned
        };
    }

    public void UpdateAssigned(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Assigned amount cannot be negative.");

        Assigned = amount;
    }
}
