using MediatR;

namespace ZeroBudget.Application.Features.Categories.DeleteCategoryGroup;

public class DeleteCategoryGroupCommandInput : IRequest<Unit>
{
    public DeleteCategoryGroupCommandInput(Guid groupId, Guid? targetCategoryId = null)
    {
        GroupId = groupId;
        TargetCategoryId = targetCategoryId;
    }

    public Guid GroupId { get; }
    public Guid? TargetCategoryId { get; }
}
