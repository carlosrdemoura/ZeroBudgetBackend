using Microsoft.EntityFrameworkCore;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Infrastructure.Persistence.Repositories;

public class CategoryRepository(AppDbContext context) : ICategoryRepository
{
    public async Task<List<CategoryGroup>> GetGroupsWithCategoriesAsync(
        CancellationToken cancellationToken = default)
        => await context.CategoryGroups
            .Where(g => !g.IsSystem)
            .Include(g => g.Categories)
            .OrderBy(g => g.SortOrder)
            .ToListAsync(cancellationToken);

    public async Task<CategoryGroup?> GetGroupByIdAsync(
        Guid groupId, CancellationToken cancellationToken = default)
        => await context.CategoryGroups
            .Where(g => g.Id == groupId)
            .Include(g => g.Categories)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<Category?> GetByIdAsync(
        Guid categoryId, CancellationToken cancellationToken = default)
        => await context.Categories
            .FirstOrDefaultAsync(
                c => c.Id == categoryId,
                cancellationToken);

    public async Task AddGroupAsync(CategoryGroup group, CancellationToken cancellationToken = default)
        => await context.CategoryGroups.AddAsync(group, cancellationToken);

    public async Task AddCategoryAsync(Category category, CancellationToken cancellationToken = default)
        => await context.Categories.AddAsync(category, cancellationToken);

    public async Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken cancellationToken = default)
        => await context.Transactions
            .AnyAsync(t => t.CategoryId == categoryId, cancellationToken);

    public async Task<List<CategoryGroup>> GetGroupsByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        => await context.CategoryGroups
            .Where(g => ids.Contains(g.Id))
            .ToListAsync(cancellationToken);

    public async Task<List<Category>> GetCategoriesByIdsAsync(
        IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        => await context.Categories
            .Where(c => ids.Contains(c.Id))
            .ToListAsync(cancellationToken);

    public Task DeleteAsync(Category category, CancellationToken cancellationToken = default)
    {
        context.Categories.Remove(category);
        return Task.CompletedTask;
    }

    public void DeleteGroup(CategoryGroup group) => context.CategoryGroups.Remove(group);

    public async Task<Category?> FindByNameAsync(string name, Guid? groupId = null, CancellationToken cancellationToken = default)
    {
        var query = context.Categories.Where(c => c.Name.ToLower() == name.ToLower());
        if (groupId.HasValue)
            query = query.Where(c => c.GroupId == groupId.Value);
        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<CategoryGroup?> GetFirstUserGroupAsync(CancellationToken cancellationToken = default)
        => await context.CategoryGroups
            .Where(g => !g.IsSystem)
            .OrderBy(g => g.SortOrder)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<bool> AnyHasTransactionsAsync(IEnumerable<Guid> categoryIds, CancellationToken cancellationToken = default)
    {
        var ids = categoryIds.ToList();
        return await context.Transactions
            .AnyAsync(t => t.CategoryId != null && ids.Contains(t.CategoryId.Value), cancellationToken);
    }
}
