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
        => new()
        {
            Id = Guid.NewGuid(),
            CategoryId = categoryId,
            Month = month,
            Assigned = assigned
        };

    public void UpdateAssigned(decimal amount) => Assigned = amount;
}
