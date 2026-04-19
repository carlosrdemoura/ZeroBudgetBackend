using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.Exceptions;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Application.Features.Categories.DeleteCategoryGroup;

public class DeleteCategoryGroupCommandHandler(
    ICategoryRepository categoryRepository,
    ITransactionRepository transactionRepository,
    IBudgetEntryRepository budgetEntryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryGroupCommandInput, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryGroupCommandInput command, CancellationToken cancellationToken)
    {
        var group = await categoryRepository.GetGroupByIdAsync(command.GroupId, cancellationToken);
        if (group is null)
            throw new NotFoundException("CategoryGroup", command.GroupId);

        if (group.IsSystem)
            throw new DomainException("System category groups cannot be deleted.");

        var categoryIds = group.Categories.Select(c => c.Id).ToList();

        if (categoryIds.Count > 0)
        {
            var sourceEntries = await budgetEntryRepository.GetByCategoryIdsAsync(categoryIds, cancellationToken);
            var hasTransactions = await categoryRepository.AnyHasTransactionsAsync(categoryIds, cancellationToken);

            if (sourceEntries.Count > 0 || hasTransactions)
            {
                if (command.TargetCategoryId is null)
                    throw new DomainException(
                        $"Group '{group.Name}' has categories with data. Provide a targetCategoryId to move them.");

                if (categoryIds.Contains(command.TargetCategoryId.Value))
                    throw new DomainException(
                        "Target category must be outside the group being deleted.");

                var target = await categoryRepository.GetByIdAsync(command.TargetCategoryId.Value, cancellationToken);
                if (target is null)
                    throw new NotFoundException("Category", command.TargetCategoryId.Value);

                if (hasTransactions)
                {
                    await transactionRepository.MoveTransactionsFromCategoriesToCategoryAsync(
                        categoryIds, target.Id, cancellationToken);
                }

                await MergeBudgetEntriesAsync(sourceEntries, target.Id, cancellationToken);
            }
        }

        categoryRepository.DeleteGroup(group);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }

    private async Task MergeBudgetEntriesAsync(
        List<BudgetEntry> sourceEntries,
        Guid targetCategoryId,
        CancellationToken cancellationToken)
    {
        var byMonth = sourceEntries
            .GroupBy(e => e.Month)
            .Select(g => (Month: g.Key, Assigned: g.Sum(e => e.Assigned)));

        foreach (var (month, assigned) in byMonth)
        {
            var targetEntry = await budgetEntryRepository.GetAsync(targetCategoryId, month, cancellationToken);
            if (targetEntry is null)
                await budgetEntryRepository.AddAsync(BudgetEntry.Create(targetCategoryId, month, assigned), cancellationToken);
            else
                targetEntry.UpdateAssigned(targetEntry.Assigned + assigned);
        }
    }
}
