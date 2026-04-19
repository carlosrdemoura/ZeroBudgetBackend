using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain;
using ZeroBudget.Domain.Services;

namespace ZeroBudget.Application.Features.Categories.GetCategoryBalances;

public class GetCategoryBalancesQueryHandler(
    ICategoryRepository categoryRepository,
    IBudgetEntryRepository budgetEntries,
    ITransactionRepository transactions) : IRequestHandler<GetCategoryBalancesQueryInput, GetCategoryBalancesQueryOutput>
{
    public async Task<GetCategoryBalancesQueryOutput> Handle(
        GetCategoryBalancesQueryInput query,
        CancellationToken cancellationToken)
    {
        var month = query.Month;
        var prevMonth = month.Previous();
        var inflowCategoryId = WellKnownIds.InflowCategoryId;

        var groups = await categoryRepository.GetGroupsWithCategoriesAsync(cancellationToken);
        var thisMonthEntries = (await budgetEntries.GetByMonthAsync(month, cancellationToken)).ToDictionary(e => e.CategoryId, e => e.Assigned);
        var thisMonthActivity = await transactions.GetActivityForMonthAsync(month, inflowCategoryId, cancellationToken);
        var prevCumAssigned = await budgetEntries.GetCumulativeAssignedPerCategoryAsync(prevMonth, cancellationToken);
        var prevCumActivity = await transactions.GetCumulativeActivityPerCategoryAsync(prevMonth, inflowCategoryId, cancellationToken);

        var results = new List<CategoryBalanceDTO>();

        foreach (var group in groups.OrderBy(g => g.SortOrder))
        {
            foreach (var category in group.Categories.OrderBy(c => c.SortOrder))
            {
                var previousBalance =
                    prevCumAssigned.GetValueOrDefault(category.Id) +
                    prevCumActivity.GetValueOrDefault(category.Id);

                var assigned = thisMonthEntries.GetValueOrDefault(category.Id);
                var activity = thisMonthActivity.GetValueOrDefault(category.Id);
                var balance = BudgetCalculationService.CalculateCategoryBalance(assigned + previousBalance, activity);

                results.Add(new CategoryBalanceDTO(
                    category.Id,
                    category.Name,
                    group.Id,
                    group.Name,
                    previousBalance,
                    assigned,
                    activity,
                    balance));
            }
        }

        return new GetCategoryBalancesQueryOutput(results);
    }
}
