using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Categories.MoveCategory;

public class MoveCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<MoveCategoryCommandInput, Unit>
{
    public async Task<Unit> Handle(MoveCategoryCommandInput command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        if (category is null)
            throw new NotFoundException("Category", command.CategoryId);

        var targetGroup = await categoryRepository.GetGroupByIdAsync(command.TargetGroupId, cancellationToken);
        if (targetGroup is null)
            throw new NotFoundException("CategoryGroup", command.TargetGroupId);

        category.Move(command.TargetGroupId);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
