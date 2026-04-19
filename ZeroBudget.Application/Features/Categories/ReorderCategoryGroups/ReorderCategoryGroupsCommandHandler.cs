using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Categories.ReorderCategoryGroups;

public class ReorderCategoryGroupsCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ReorderCategoryGroupsCommandInput, Unit>
{
    public async Task<Unit> Handle(ReorderCategoryGroupsCommandInput command, CancellationToken cancellationToken)
    {
        var ids = command.Items.Select(i => i.GroupId);
        var groups = await categoryRepository.GetGroupsByIdsAsync(ids, cancellationToken);
        var groupMap = groups.ToDictionary(g => g.Id);

        foreach (var item in command.Items)
        {
            if (!groupMap.TryGetValue(item.GroupId, out var group))
                throw new NotFoundException("CategoryGroup", item.GroupId);

            group.SetSortOrder(item.SortOrder);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
