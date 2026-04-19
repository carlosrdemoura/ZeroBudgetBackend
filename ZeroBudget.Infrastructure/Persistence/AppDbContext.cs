using Microsoft.EntityFrameworkCore;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<CategoryGroup> CategoryGroups { get; set; } = null!;
    public DbSet<Category> Categories { get; set; } = null!;
    public DbSet<BudgetEntry> BudgetEntries { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
