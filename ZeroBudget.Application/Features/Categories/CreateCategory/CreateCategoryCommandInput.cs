using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Categories.CreateCategory;

public class CreateCategoryCommandInput : IRequest<CreateCategoryCommandOutput>
{
    public CreateCategoryCommandInput(Guid groupId, string name)
    {
        GroupId = groupId;
        Name = name;
    }

    public Guid GroupId { get; }
    public string Name { get; }
}

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommandInput>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.GroupId).NotEmpty();
    }
}
