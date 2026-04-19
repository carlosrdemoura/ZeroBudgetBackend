using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Budget.AssignAmount;

public class AssignAmountCommandHandler(
    IBudgetEntryRepository budgetEntries,
    ICategoryRepository categories,
    IUnitOfWork unitOfWork) : IRequestHandler<AssignAmountCommandInput, Unit>
{
    public async Task<Unit> Handle(AssignAmountCommandInput command, CancellationToken cancellationToken)
    {
        var (month, categoryId, amount) = command;

        var category = await categories.GetByIdAsync(categoryId, cancellationToken);
        if (category is null)
            throw new NotFoundException(nameof(Category), categoryId);

        await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var currentEntry = await budgetEntries.GetAsync(categoryId, month, cancellationToken);

            if (currentEntry is null)
                await budgetEntries.AddAsync(BudgetEntry.Create(categoryId, month, amount), cancellationToken);
            else
                currentEntry.UpdateAssigned(amount);

            await unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        return Unit.Value;
    }
}
