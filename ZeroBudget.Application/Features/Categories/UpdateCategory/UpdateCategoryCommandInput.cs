using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Categories.UpdateCategory;

public class UpdateCategoryCommandInput : IRequest<Unit>
{
    public UpdateCategoryCommandInput(Guid categoryId, string name)
    {
        CategoryId = categoryId;
        Name = name;
    }

    public Guid CategoryId { get; }
    public string Name { get; }
}

public class UpdateCategoryCommandValidator : AbstractValidator<UpdateCategoryCommandInput>
{
    public UpdateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
