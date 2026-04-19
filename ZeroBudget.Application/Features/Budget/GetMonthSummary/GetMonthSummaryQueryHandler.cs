using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain;
using ZeroBudget.Domain.Services;

namespace ZeroBudget.Application.Features.Budget.GetMonthSummary;

public class GetMonthSummaryQueryHandler(
    ITransactionRepository transactions,
    IBudgetEntryRepository budgetEntries) : IRequestHandler<GetMonthSummaryQueryInput, GetMonthSummaryQueryOutput>
{
    public async Task<GetMonthSummaryQueryOutput> Handle(
        GetMonthSummaryQueryInput query,
        CancellationToken cancellationToken)
    {
        var month = query.Month;
        var inflowCategoryId = WellKnownIds.InflowCategoryId;

        var income = await transactions.GetMonthlyIncomeAsync(month, inflowCategoryId, cancellationToken);
        var assigned = await budgetEntries.GetTotalAssignedForMonthAsync(month, cancellationToken);

        var prevMonth = month.Previous();
        var prevCumIncome = await transactions.GetCumulativeIncomeAsync(prevMonth, inflowCategoryId, cancellationToken);
        var prevCumAssigned = (await budgetEntries.GetCumulativeAssignedPerCategoryAsync(prevMonth, cancellationToken)).Values.Sum();

        var rollover = prevCumIncome - prevCumAssigned;
        var available = BudgetCalculationService.CalculateAvailableToAssign(income, assigned) + rollover;

        return new GetMonthSummaryQueryOutput(
            new MonthSummaryDTO(month.ToString(), income, assigned, available, rollover));
    }
}
