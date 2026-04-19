using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;

namespace ZeroBudget.Application.Features.Categories.GetCategoryGroups;

public class GetCategoryGroupsQueryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<GetCategoryGroupsQueryInput, GetCategoryGroupsQueryOutput>
{
    public async Task<GetCategoryGroupsQueryOutput> Handle(
        GetCategoryGroupsQueryInput query,
        CancellationToken cancellationToken)
    {
        var groups = await categoryRepository.GetGroupsWithCategoriesAsync(cancellationToken);

        var result = groups
            .OrderBy(g => g.SortOrder)
            .Select(g => new CategoryGroupDTO(
                g.Id,
                g.Name,
                g.SortOrder,
                g.Categories
                    .OrderBy(c => c.SortOrder)
                    .Select(c => new CategoryDTO(c.Id, c.GroupId, c.Name, c.SortOrder))
                    .ToList()))
            .ToList();

        return new GetCategoryGroupsQueryOutput(result);
    }
}
