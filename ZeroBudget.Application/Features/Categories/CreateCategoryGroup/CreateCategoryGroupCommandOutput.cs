using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Categories.CreateCategoryGroup;

public class CreateCategoryGroupCommandOutput
{
    public CreateCategoryGroupCommandOutput(CategoryGroupDTO categoryGroup)
    {
        CategoryGroup = categoryGroup;
    }

    public CategoryGroupDTO CategoryGroup { get; }
}
