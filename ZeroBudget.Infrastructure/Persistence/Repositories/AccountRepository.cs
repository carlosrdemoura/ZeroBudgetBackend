using Microsoft.EntityFrameworkCore;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Infrastructure.Persistence.Repositories;

public class AccountRepository(AppDbContext context) : IAccountRepository
{
    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await context.Accounts
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task<List<Account>> GetAllAsync(CancellationToken cancellationToken = default)
        => await context.Accounts
            .OrderBy(a => a.Name)
            .ToListAsync(cancellationToken);

    public async Task<bool> HasExpenseTransactionsAsync(Guid accountId, CancellationToken cancellationToken = default)
        => await context.Transactions
            .AnyAsync(t =>
                t.AccountId == accountId &&
                t.CategoryId != WellKnownIds.InflowCategoryId,
                cancellationToken);

    public async Task AddAsync(Account account, CancellationToken cancellationToken = default)
        => await context.Accounts.AddAsync(account, cancellationToken);

    public Task DeleteAsync(Account account, CancellationToken cancellationToken = default)
    {
        context.Accounts.Remove(account);
        return Task.CompletedTask;
    }
}
