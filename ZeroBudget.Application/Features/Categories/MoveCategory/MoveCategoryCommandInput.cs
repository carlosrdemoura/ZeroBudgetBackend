using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Categories.MoveCategory;

public class MoveCategoryCommandInput : IRequest<Unit>
{
    public MoveCategoryCommandInput(Guid categoryId, Guid targetGroupId)
    {
        CategoryId = categoryId;
        TargetGroupId = targetGroupId;
    }

    public Guid CategoryId { get; }
    public Guid TargetGroupId { get; }
}

public class MoveCategoryCommandValidator : AbstractValidator<MoveCategoryCommandInput>
{
    public MoveCategoryCommandValidator()
    {
        RuleFor(x => x.TargetGroupId).NotEmpty();
    }
}
