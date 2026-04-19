using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain;

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
        var totalIncome = await transactionRepository.GetTotalIncomeAsync(WellKnownIds.InflowCategoryId, cancellationToken);

        var readyToAssign = totalIncome - totalAssigned;

        return new GetAccountsSummaryQueryOutput(totalBalance, totalAssigned, readyToAssign);
    }
}
