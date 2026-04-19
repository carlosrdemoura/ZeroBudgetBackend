using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Budget.MoveMoney;

public class MoveMoneyCommandHandler(
    IBudgetEntryRepository budgetEntries,
    ICategoryRepository categories,
    IUnitOfWork unitOfWork) : IRequestHandler<MoveMoneyCommandInput, Unit>
{
    public async Task<Unit> Handle(MoveMoneyCommandInput command, CancellationToken cancellationToken)
    {
        var (month, fromCategoryId, toCategoryId, amount) = command;

        var fromCategory = await categories.GetByIdAsync(fromCategoryId, cancellationToken);
        if (fromCategory is null)
            throw new NotFoundException(nameof(Category), fromCategoryId);

        var toCategory = await categories.GetByIdAsync(toCategoryId, cancellationToken);
        if (toCategory is null)
            throw new NotFoundException(nameof(Category), toCategoryId);

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var fromEntry = await budgetEntries.GetAsync(fromCategoryId, month, cancellationToken);
            var fromAssigned = fromEntry?.Assigned ?? 0;

            if (fromEntry is null)
                await budgetEntries.AddAsync(BudgetEntry.Create(fromCategoryId, month, 0), cancellationToken);
            else
                fromEntry.UpdateAssigned(fromAssigned - amount);

            var toEntry = await budgetEntries.GetAsync(toCategoryId, month, cancellationToken);
            var toAssigned = toEntry?.Assigned ?? 0;

            if (toEntry is null)
                await budgetEntries.AddAsync(BudgetEntry.Create(toCategoryId, month, amount), cancellationToken);
            else
                toEntry.UpdateAssigned(toAssigned + amount);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return Unit.Value;
    }
}
