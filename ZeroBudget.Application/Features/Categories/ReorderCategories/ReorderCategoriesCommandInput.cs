using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Categories.ReorderCategories;

public record CategorySortItem(Guid CategoryId, int SortOrder);

public class ReorderCategoriesCommandInput : IRequest<Unit>
{
    public ReorderCategoriesCommandInput(Guid groupId, IEnumerable<CategorySortItem> items)
    {
        GroupId = groupId;
        Items = items.ToList();
    }

    public Guid GroupId { get; }
    public List<CategorySortItem> Items { get; }
}

public class ReorderCategoriesCommandValidator : AbstractValidator<ReorderCategoriesCommandInput>
{
    public ReorderCategoriesCommandValidator()
    {
        RuleFor(x => x.Items).NotEmpty();
        RuleForEach(x => x.Items).ChildRules(item =>
        {
            item.RuleFor(x => x.CategoryId).NotEmpty();
            item.RuleFor(x => x.SortOrder).GreaterThanOrEqualTo(0);
        });
    }
}
