using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.Exceptions;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Application.Features.Categories.DeleteCategory;

public class DeleteCategoryCommandHandler(
    ICategoryRepository categoryRepository,
    ITransactionRepository transactionRepository,
    IBudgetEntryRepository budgetEntryRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCategoryCommandInput, Unit>
{
    public async Task<Unit> Handle(DeleteCategoryCommandInput command, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(command.CategoryId, cancellationToken);
        if (category is null)
            throw new NotFoundException("Category", command.CategoryId);

        if (category.IsSystem)
            throw new DomainException("System categories cannot be deleted.");

        var sourceEntries = await budgetEntryRepository.GetByCategoryIdsAsync([category.Id], cancellationToken);
        var hasTransactions = await categoryRepository.HasTransactionsAsync(category.Id, cancellationToken);

        if (sourceEntries.Count > 0 || hasTransactions)
        {
            if (command.TargetCategoryId is null)
                throw new DomainException(
                    $"Category '{category.Name}' has data. Provide a targetCategoryId to move it.");

            if (command.TargetCategoryId.Value == category.Id)
                throw new DomainException("Target category must be different from the category being deleted.");

            var target = await categoryRepository.GetByIdAsync(command.TargetCategoryId.Value, cancellationToken);
            if (target is null)
                throw new NotFoundException("Category", command.TargetCategoryId.Value);

            if (hasTransactions)
            {
                await transactionRepository.MoveTransactionsToCategoryAsync(category.Id, target.Id, cancellationToken);
            }

            await MergeBudgetEntriesAsync(sourceEntries, target.Id, cancellationToken);
        }

        await categoryRepository.DeleteAsync(category, cancellationToken);
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
