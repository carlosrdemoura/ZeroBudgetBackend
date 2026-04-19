using FluentValidation;
using MediatR;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Application.Features.Budget.AssignAmount;

public class AssignAmountCommandInput : IRequest<Unit>
{
    public AssignAmountCommandInput(YearMonth month, Guid categoryId, decimal amount)
    {
        Month = month;
        CategoryId = categoryId;
        Amount = amount;
    }

    public YearMonth Month { get; }
    public Guid CategoryId { get; }
    public decimal Amount { get; }

    public void Deconstruct(out YearMonth month, out Guid categoryId, out decimal amount)
    {
        month = Month;
        categoryId = CategoryId;
        amount = Amount;
    }
}

public class AssignAmountCommandValidator : AbstractValidator<AssignAmountCommandInput>
{
    public AssignAmountCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Assigned amount cannot be negative.");

        RuleFor(x => x.CategoryId).NotEmpty();
    }
}
