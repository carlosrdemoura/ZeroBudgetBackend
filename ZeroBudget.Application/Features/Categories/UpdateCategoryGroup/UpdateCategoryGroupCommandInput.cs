using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Categories.UpdateCategoryGroup;

public class UpdateCategoryGroupCommandInput : IRequest<Unit>
{
    public UpdateCategoryGroupCommandInput(Guid groupId, string name)
    {
        GroupId = groupId;
        Name = name;
    }

    public Guid GroupId { get; }
    public string Name { get; }
}

public class UpdateCategoryGroupCommandValidator : AbstractValidator<UpdateCategoryGroupCommandInput>
{
    public UpdateCategoryGroupCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
