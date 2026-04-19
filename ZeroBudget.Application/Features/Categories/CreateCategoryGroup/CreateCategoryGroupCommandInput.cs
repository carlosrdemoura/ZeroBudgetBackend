using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Categories.CreateCategoryGroup;

public class CreateCategoryGroupCommandInput : IRequest<CreateCategoryGroupCommandOutput>
{
    public CreateCategoryGroupCommandInput(string name)
    {
        Name = name;
    }

    public string Name { get; }
}

public class CreateCategoryGroupCommandValidator : AbstractValidator<CreateCategoryGroupCommandInput>
{
    public CreateCategoryGroupCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
