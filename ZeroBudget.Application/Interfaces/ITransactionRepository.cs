using ZeroBudget.Application.Common;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Application.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<PagedResult<Transaction>> GetPagedAsync(
        int year,
        int month,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default);

    Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default);
    Task DeleteAsync(Transaction transaction, CancellationToken cancellationToken = default);
}
