namespace ZeroBudget.Domain.Entities;

public class Category
{
    public Guid Id { get; private set; }
    public Guid GroupId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public bool IsSystem { get; private set; }

    public CategoryGroup Group { get; private set; } = null!;

    private readonly List<BudgetEntry> _budgetEntries = [];
    public IEnumerable<BudgetEntry> BudgetEntries => _budgetEntries.AsReadOnly();

    private readonly List<Transaction> _transactions = [];
    public IEnumerable<Transaction> Transactions => _transactions.AsReadOnly();

    // EF Core constructor
    private Category() { }

    public static Category Create(Guid groupId, string name, int sortOrder = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new Category
        {
            Id = Guid.NewGuid(),
            GroupId = groupId,
            Name = name.Trim(),
            SortOrder = sortOrder,
            IsSystem = false
        };
    }

    public static Category CreateSystem(Guid groupId, string name, Guid? id = null)
        => new()
        {
            Id = id ?? Guid.NewGuid(),
            GroupId = groupId,
            Name = name,
            SortOrder = 0,
            IsSystem = true
        };

    public void Rename(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        Name = name.Trim();
    }

    public void SetSortOrder(int order) => SortOrder = order;

    public void Move(Guid newGroupId) => GroupId = newGroupId;
}
