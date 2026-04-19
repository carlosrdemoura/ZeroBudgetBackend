using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Application.Interfaces;

public interface IBudgetEntryRepository
{
    Task<BudgetEntry?> GetAsync(Guid categoryId, YearMonth month, CancellationToken cancellationToken = default);
    Task<List<BudgetEntry>> GetByMonthAsync(YearMonth month, CancellationToken cancellationToken = default);
    Task<List<BudgetEntry>> GetByCategoryIdsAsync(IEnumerable<Guid> categoryIds, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAssignedForMonthAsync(YearMonth month, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, decimal>> GetCumulativeAssignedPerCategoryAsync(YearMonth upToMonth, CancellationToken cancellationToken = default);
    Task AddAsync(BudgetEntry entry, CancellationToken cancellationToken = default);
    Task<bool> AnyExistForMonthAsync(YearMonth month, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAllTimeBudgetAssignedAsync(CancellationToken cancellationToken = default);
}
