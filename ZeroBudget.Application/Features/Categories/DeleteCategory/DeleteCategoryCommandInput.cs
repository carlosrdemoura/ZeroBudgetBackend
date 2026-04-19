using MediatR;

namespace ZeroBudget.Application.Features.Categories.DeleteCategory;

public class DeleteCategoryCommandInput : IRequest<Unit>
{
    public DeleteCategoryCommandInput(Guid categoryId, Guid? targetCategoryId = null)
    {
        CategoryId = categoryId;
        TargetCategoryId = targetCategoryId;
    }

    public Guid CategoryId { get; }
    public Guid? TargetCategoryId { get; }
}
