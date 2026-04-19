using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Categories.ReorderCategories;

public class ReorderCategoriesCommandHandler(
    ICategoryRepository categoryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ReorderCategoriesCommandInput, Unit>
{
    public async Task<Unit> Handle(ReorderCategoriesCommandInput command, CancellationToken cancellationToken)
    {
        var group = await categoryRepository.GetGroupByIdAsync(command.GroupId, cancellationToken);
        if (group is null)
            throw new NotFoundException("CategoryGroup", command.GroupId);

        var ids = command.Items.Select(i => i.CategoryId);
        var categories = await categoryRepository.GetCategoriesByIdsAsync(ids, cancellationToken);
        var categoryMap = categories.ToDictionary(c => c.Id);

        foreach (var item in command.Items)
        {
            if (!categoryMap.TryGetValue(item.CategoryId, out var category))
                throw new NotFoundException("Category", item.CategoryId);

            if (category.GroupId != command.GroupId)
                throw new DomainException(
                    $"Category '{item.CategoryId}' does not belong to group '{command.GroupId}'.");

            category.SetSortOrder(item.SortOrder);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);
        return Unit.Value;
    }
}
