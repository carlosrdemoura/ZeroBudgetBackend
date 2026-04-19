using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Categories.CreateCategory;

public class CreateCategoryCommandOutput
{
    public CreateCategoryCommandOutput(CategoryDTO category)
    {
        Category = category;
    }

    public CategoryDTO Category { get; }
}
