using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Categories.GetCategoryGroups;

public class GetCategoryGroupsQueryOutput
{
    public GetCategoryGroupsQueryOutput(IEnumerable<CategoryGroupDTO> categoryGroups)
    {
        CategoryGroups = categoryGroups;
    }

    public IEnumerable<CategoryGroupDTO> CategoryGroups { get; }
}
