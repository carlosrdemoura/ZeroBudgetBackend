using Microsoft.EntityFrameworkCore;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Infrastructure.Persistence.Repositories;

public class BudgetEntryRepository(AppDbContext context) : IBudgetEntryRepository
{
    public async Task<BudgetEntry?> GetAsync(
        Guid categoryId, YearMonth month, CancellationToken cancellationToken = default)
        => await context.BudgetEntries
            .FirstOrDefaultAsync(
                e => e.CategoryId == categoryId && e.Month == month,
                cancellationToken);

    public async Task<List<BudgetEntry>> GetByMonthAsync(
        YearMonth month, CancellationToken cancellationToken = default)
        => await context.BudgetEntries
            .Where(e => e.Month == month)
            .ToListAsync(cancellationToken);

    public async Task<List<BudgetEntry>> GetByCategoryIdsAsync(
        IEnumerable<Guid> categoryIds, CancellationToken cancellationToken = default)
    {
        var ids = categoryIds.ToList();
        return await context.BudgetEntries
            .Where(e => ids.Contains(e.CategoryId))
            .ToListAsync(cancellationToken);
    }

    public async Task<decimal> GetTotalAssignedForMonthAsync(
        YearMonth month, CancellationToken cancellationToken = default)
        => await context.BudgetEntries
            .Where(e => e.Month == month)
            .SumAsync(e => e.Assigned, cancellationToken);

    public async Task<Dictionary<Guid, decimal>> GetCumulativeAssignedPerCategoryAsync(
        YearMonth upToMonth, CancellationToken cancellationToken = default)
    {
        var allEntries = await context.BudgetEntries
            .ToListAsync(cancellationToken);

        return allEntries
            .Where(e => !e.Month.IsAfter(upToMonth))
            .GroupBy(e => e.CategoryId)
            .ToDictionary(g => g.Key, g => g.Sum(e => e.Assigned));
    }

    public async Task AddAsync(BudgetEntry entry, CancellationToken cancellationToken = default)
        => await context.BudgetEntries.AddAsync(entry, cancellationToken);

    public async Task<bool> AnyExistForMonthAsync(
        YearMonth month, CancellationToken cancellationToken = default)
        => await context.BudgetEntries
            .AnyAsync(e => e.Month == month, cancellationToken);

    public async Task<decimal> GetTotalAllTimeBudgetAssignedAsync(
        CancellationToken cancellationToken = default)
        => await context.BudgetEntries
            .SumAsync(e => e.Assigned, cancellationToken);
}
