using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Categories.ReorderCategoryGroups;

public record GroupSortItem(Guid GroupId, int SortOrder);

public class ReorderCategoryGroupsCommandInput : IRequest<Unit>
{
    public ReorderCategoryGroupsCommandInput(IEnumerable<GroupSortItem> items)
    {
        Items = items.ToList();
    }

    public List<GroupSortItem> Items { get; }
}

public class ReorderCategoryGroupsCommandValidator : AbstractValidator<ReorderCategoryGroupsCommandInput>
{
    public ReorderCategoryGroupsCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.GroupId).NotEmpty();
            item.RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        });
    }
}
