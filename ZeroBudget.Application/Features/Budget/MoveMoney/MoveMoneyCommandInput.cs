using FluentValidation;
using MediatR;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Application.Features.Budget.MoveMoney;

public class MoveMoneyCommandInput : IRequest<Unit>
{
    public MoveMoneyCommandInput(YearMonth month, Guid fromCategoryId, Guid toCategoryId, decimal amount)
    {
        Month = month;
        FromCategoryId = fromCategoryId;
        ToCategoryId = toCategoryId;
        Amount = amount;
    }

    public YearMonth Month { get; }
    public Guid FromCategoryId { get; }
    public Guid ToCategoryId { get; }
    public decimal Amount { get; }

    public void Deconstruct(out YearMonth month, out Guid fromCategoryId, out Guid toCategoryId, out decimal amount)
    {
        month = Month;
        fromCategoryId = FromCategoryId;
        toCategoryId = ToCategoryId;
        amount = Amount;
    }
}

public class MoveMoneyCommandValidator : AbstractValidator<MoveMoneyCommandInput>
{
    public MoveMoneyCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount to move must be greater than zero.");

        RuleFor(x => x.ToCategoryId)
            .NotEqual(x => x.FromCategoryId)
            .WithMessage("Source and destination categories must be different.");
    }
}
