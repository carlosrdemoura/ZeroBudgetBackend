using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Transactions.CreateTransaction;

public class CreateTransactionCommandInput : IRequest<CreateTransactionCommandOutput>
{
    public CreateTransactionCommandInput(Guid accountId, decimal amount, DateOnly date, Guid? categoryId, string? categoryName, Guid? categoryGroupId, string? memo, bool affectsBudget = true)
    {
        AccountId = accountId;
        Amount = amount;
        Date = date;
        CategoryId = categoryId;
        CategoryName = categoryName;
        CategoryGroupId = categoryGroupId;
        Memo = memo;
        AffectsBudget = affectsBudget;
    }

    public Guid AccountId { get; }
    public decimal Amount { get; }
    public DateOnly Date { get; }
    public Guid? CategoryId { get; }
    public string? CategoryName { get; }
    public Guid? CategoryGroupId { get; }
    public string? Memo { get; }
    public bool AffectsBudget { get; }
}

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommandInput>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.AccountId).NotEmpty().WithMessage("Account is required.");
        RuleFor(x => x.Amount).NotEqual(0).WithMessage("Amount cannot be zero.");
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.CategoryName).MaximumLength(100).When(x => x.CategoryName is not null);
        RuleFor(x => x.Memo).MaximumLength(500).When(x => x.Memo is not null);
    }
}
