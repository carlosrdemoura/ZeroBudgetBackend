using Microsoft.EntityFrameworkCore;
using ZeroBudget.Application.Common;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Infrastructure.Persistence.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<Transaction?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<PagedResult<Transaction>> GetPagedAsync(
        int year,
        int month,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var fromDate = new DateOnly(year, month, 1);
        var toDate = fromDate.AddMonths(1);

        var query = context.Transactions
            .Where(t => t.Date >= fromDate && t.Date < toDate);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(t =>
                EF.Functions.ILike(EF.Functions.Unaccent(t.Description ?? ""), EF.Functions.Unaccent(pattern)));
        }

        query = query.OrderByDescending(t => t.Date).ThenByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Transaction>(items, totalCount, page, pageSize);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
        => await context.Transactions.AddAsync(transaction, cancellationToken);

    public Task DeleteAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        context.Transactions.Remove(transaction);
        return Task.CompletedTask;
    }
}
