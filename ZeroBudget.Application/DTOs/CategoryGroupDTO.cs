namespace ZeroBudget.Application.DTOs;

public class CategoryGroupDTO
{
    public CategoryGroupDTO(Guid id, string name, int sortOrder, IEnumerable<CategoryDTO> categories)
    {
        Id = id;
        Name = name;
        SortOrder = sortOrder;
        Categories = categories;
    }

    public Guid Id { get; }
    public string Name { get; }
    public int SortOrder { get; }
    public IEnumerable<CategoryDTO> Categories { get; }
}
