using ZeroBudget.Application.Common;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Application.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Transaction>> GetPagedAsync(
        Guid accountId,
        DateOnly fromDate,
        DateOnly toDate,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task<decimal> GetMonthlyIncomeAsync(YearMonth month, Guid inflowCategoryId, CancellationToken cancellationToken = default);
    Task<decimal> GetCumulativeIncomeAsync(YearMonth upToMonth, Guid inflowCategoryId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, decimal>> GetActivityForMonthAsync(YearMonth month, Guid inflowCategoryId, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, decimal>> GetCumulativeActivityPerCategoryAsync(YearMonth upToMonth, Guid inflowCategoryId, CancellationToken cancellationToken = default);
    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task<Dictionary<Guid, decimal>> GetAccountBalancesAsync(CancellationToken cancellationToken = default);
    Task DeleteByAccountAsync(Guid accountId, CancellationToken cancellationToken = default);
    Task MoveTransactionsToCategoryAsync(Guid fromCategoryId, Guid toCategoryId, CancellationToken cancellationToken = default);
    Task MoveTransactionsFromCategoriesToCategoryAsync(IEnumerable<Guid> fromCategoryIds, Guid toCategoryId, CancellationToken cancellationToken = default);
}
