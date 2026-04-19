using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Categories.UpdateCategoryGroup;

public class UpdateCategoryGroupCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCategoryGroupCommandInput, Unit>
{
    public async Task<Unit> Handle(UpdateCategoryGroupCommandInput command, CancellationToken cancellationToken)
    {
        var group = await categoryRepository.GetGroupByIdAsync(command.GroupId, cancellationToken);
        if (group is null)
            throw new NotFoundException("CategoryGroup", command.GroupId);

        group.Rename(command.Name);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
