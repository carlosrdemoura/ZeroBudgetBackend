using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Application.Interfaces;

public interface ICategoryRepository
{
    Task<List<CategoryGroup>> GetGroupsWithCategoriesAsync(CancellationToken cancellationToken = default);
    Task<CategoryGroup?> GetGroupByIdAsync(Guid groupId, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task AddGroupAsync(CategoryGroup group, CancellationToken cancellationToken = default);
    Task AddCategoryAsync(Category category, CancellationToken cancellationToken = default);
    Task<bool> HasTransactionsAsync(Guid categoryId, CancellationToken cancellationToken = default);
    Task<List<CategoryGroup>> GetGroupsByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task<List<Category>> GetCategoriesByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    Task DeleteAsync(Category category, CancellationToken cancellationToken = default);
    Task<Category?> FindByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<CategoryGroup?> GetFirstUserGroupAsync(CancellationToken cancellationToken = default);
    void DeleteGroup(CategoryGroup group);
    Task<bool> AnyHasTransactionsAsync(IEnumerable<Guid> categoryIds, CancellationToken cancellationToken = default);
}
