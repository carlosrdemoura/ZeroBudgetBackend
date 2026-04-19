namespace ZeroBudget.Application.DTOs;

public class CategoryDTO
{
    public CategoryDTO(Guid id, Guid groupId, string name, int sortOrder)
    {
        Id = id;
        GroupId = groupId;
        Name = name;
        SortOrder = sortOrder;
    }

    public Guid Id { get; }
    public Guid GroupId { get; }
    public string Name { get; }
    public int SortOrder { get; }
}
