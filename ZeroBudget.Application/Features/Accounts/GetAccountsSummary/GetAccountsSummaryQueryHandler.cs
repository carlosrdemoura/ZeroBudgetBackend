using MediatR;
using ZeroBudget.Application.Interfaces;

namespace ZeroBudget.Application.Features.Accounts.GetAccountsSummary;

public class GetAccountsSummaryQueryHandler(
    ITransactionRepository transactionRepository,
    IBudgetEntryRepository budgetEntryRepository) : IRequestHandler<GetAccountsSummaryQueryInput, GetAccountsSummaryQueryOutput>
{
    public async Task<GetAccountsSummaryQueryOutput> Handle(GetAccountsSummaryQueryInput query, CancellationToken cancellationToken)
    {
        var balances = await transactionRepository.GetAccountBalancesAsync(cancellationToken);
        var totalBalance = balances.Values.Sum();

        var totalAssigned = await budgetEntryRepository.GetTotalAllTimeBudgetAssignedAsync(cancellationToken);

        var readyToAssign = totalBalance - totalAssigned;

        return new GetAccountsSummaryQueryOutput(totalBalance, totalAssigned, readyToAssign);
    }
}
