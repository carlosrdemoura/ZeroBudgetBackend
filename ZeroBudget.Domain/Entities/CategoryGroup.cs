namespace ZeroBudget.Domain.Entities;

public class CategoryGroup
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public int SortOrder { get; private set; }
    public bool IsSystem { get; private set; }

    private readonly List<Category> _categories = [];
    public IEnumerable<Category> Categories => _categories.AsReadOnly();

    // EF Core constructor
    private CategoryGroup() { }

    public static CategoryGroup Create(string name, int sortOrder = 0)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        return new CategoryGroup
        {
            Id = Guid.NewGuid(),
            Name = name.Trim(),
            SortOrder = sortOrder,
            IsSystem = false
        };
    }

    public static CategoryGroup CreateSystem(string name)
        => new()
        {
            Id = Guid.NewGuid(),
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
}
