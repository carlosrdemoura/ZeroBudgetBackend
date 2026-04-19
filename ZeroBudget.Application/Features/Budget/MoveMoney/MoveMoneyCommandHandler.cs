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

            if (fromEntry is null)
                await budgetEntries.AddAsync(BudgetEntry.Create(fromCategoryId, month, -amount), cancellationToken);
            else
                fromEntry.UpdateAssigned(fromEntry.Assigned - amount);

            var toEntry = await budgetEntries.GetAsync(toCategoryId, month, cancellationToken);

            if (toEntry is null)
                await budgetEntries.AddAsync(BudgetEntry.Create(toCategoryId, month, amount), cancellationToken);
            else
                toEntry.UpdateAssigned(toEntry.Assigned + amount);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return Unit.Value;
    }
}
