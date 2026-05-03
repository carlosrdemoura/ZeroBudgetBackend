using Microsoft.EntityFrameworkCore;
using ZeroBudget.Application.Common;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Infrastructure.Persistence.Repositories;

public class TransactionRepository(AppDbContext context) : ITransactionRepository
{
    public async Task<Transaction?> GetByIdAsync(
        Guid id, CancellationToken cancellationToken = default)
        => await context.Transactions
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

    public async Task<PagedResult<Transaction>> GetPagedAsync(
        Guid accountId,
        DateOnly fromDate,
        DateOnly toDate,
        string? search,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = context.Transactions
            .Where(t => t.AccountId == accountId &&
                        t.Date >= fromDate &&
                        t.Date <= toDate);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var pattern = $"%{search}%";
            query = query.Where(t =>
                EF.Functions.ILike(EF.Functions.Unaccent(t.Memo ?? ""), EF.Functions.Unaccent(pattern)) ||
                (t.Category != null && EF.Functions.ILike(EF.Functions.Unaccent(t.Category.Name), EF.Functions.Unaccent(pattern))) ||
                (t.Category != null && EF.Functions.ILike(EF.Functions.Unaccent(t.Category.Group.Name), EF.Functions.Unaccent(pattern))));
        }

        query = query.OrderByDescending(t => t.Date).ThenByDescending(t => t.CreatedAt);

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Transaction>(items, totalCount, page, pageSize);
    }

    public async Task<decimal> GetMonthlyIncomeAsync(
        YearMonth month, Guid inflowCategoryId, CancellationToken cancellationToken = default)
    {
        var startDate = month.FirstDay();
        var endDate = month.FirstDayOfNext();

        return await context.Transactions
            .Where(t => t.CategoryId == inflowCategoryId &&
                        t.AffectsBudget &&
                        t.Date >= startDate &&
                        t.Date < endDate)
            .SumAsync(t => t.Amount, cancellationToken);
    }

    public async Task<decimal> GetCumulativeIncomeAsync(
        YearMonth upToMonth, Guid inflowCategoryId, CancellationToken cancellationToken = default)
    {
        var endDate = upToMonth.FirstDayOfNext();

        return await context.Transactions
            .Where(t => t.CategoryId == inflowCategoryId &&
                        t.AffectsBudget &&
                        t.Date < endDate)
            .SumAsync(t => t.Amount, cancellationToken);
    }

    public async Task<decimal> GetTotalIncomeAsync(
        Guid inflowCategoryId, CancellationToken cancellationToken = default)
        => await context.Transactions
            .Where(t => t.CategoryId == inflowCategoryId && t.AffectsBudget)
            .SumAsync(t => t.Amount, cancellationToken);

    public async Task<Dictionary<Guid, decimal>> GetActivityForMonthAsync(
        YearMonth month, Guid inflowCategoryId, CancellationToken cancellationToken = default)
    {
        var startDate = month.FirstDay();
        var endDate = month.FirstDayOfNext();

        var results = await context.Transactions
            .Where(t => t.CategoryId != null &&
                        t.CategoryId != inflowCategoryId &&
                        t.AffectsBudget &&
                        t.Date >= startDate &&
                        t.Date < endDate)
            .GroupBy(t => t.CategoryId!.Value)
            .Select(g => new { CategoryId = g.Key, Activity = g.Sum(t => t.Amount) })
            .ToListAsync(cancellationToken);

        return results.ToDictionary(x => x.CategoryId, x => x.Activity);
    }

    public async Task<Dictionary<Guid, decimal>> GetCumulativeActivityPerCategoryAsync(
        YearMonth upToMonth, Guid inflowCategoryId, CancellationToken cancellationToken = default)
    {
        var endDate = upToMonth.FirstDayOfNext();

        var results = await context.Transactions
            .Where(t => t.CategoryId != null &&
                        t.CategoryId != inflowCategoryId &&
                        t.AffectsBudget &&
                        t.Date < endDate)
            .GroupBy(t => t.CategoryId!.Value)
            .Select(g => new { CategoryId = g.Key, Activity = g.Sum(t => t.Amount) })
            .ToListAsync(cancellationToken);

        return results.ToDictionary(x => x.CategoryId, x => x.Activity);
    }

    public async Task AddAsync(Transaction transaction, CancellationToken cancellationToken = default)
        => await context.Transactions.AddAsync(transaction, cancellationToken);

    public Task DeleteAsync(Transaction transaction, CancellationToken cancellationToken = default)
    {
        context.Transactions.Remove(transaction);
        return Task.CompletedTask;
    }

    public async Task<Dictionary<Guid, decimal>> GetAccountBalancesAsync(CancellationToken cancellationToken = default)
    {
        var results = await context.Transactions
            .Where(t => t.AffectsBudget)
            .GroupBy(t => t.AccountId)
            .Select(g => new { AccountId = g.Key, Balance = g.Sum(t => t.Amount) })
            .ToListAsync(cancellationToken);

        return results.ToDictionary(x => x.AccountId, x => x.Balance);
    }

    public async Task DeleteByAccountAsync(Guid accountId, CancellationToken cancellationToken = default)
    {
        var txns = await context.Transactions
            .Where(t => t.AccountId == accountId)
            .ToListAsync(cancellationToken);
        context.Transactions.RemoveRange(txns);
    }

    public async Task MoveTransactionsToCategoryAsync(Guid fromCategoryId, Guid toCategoryId, CancellationToken cancellationToken = default)
    {
        await context.Transactions
            .Where(t => t.CategoryId == fromCategoryId)
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.CategoryId, toCategoryId), cancellationToken);
    }

    public async Task MoveTransactionsFromCategoriesToCategoryAsync(IEnumerable<Guid> fromCategoryIds, Guid toCategoryId, CancellationToken cancellationToken = default)
    {
        var ids = fromCategoryIds.ToList();
        await context.Transactions
            .Where(t => ids.Contains(t.CategoryId!.Value))
            .ExecuteUpdateAsync(s => s.SetProperty(t => t.CategoryId, toCategoryId), cancellationToken);
    }
}
