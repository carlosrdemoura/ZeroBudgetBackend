using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandInput : IRequest<UpdateTransactionCommandOutput>
{
    public UpdateTransactionCommandInput(Guid transactionId, decimal amount, DateOnly date, Guid? categoryId, string? memo, bool affectsBudget = true)
    {
        TransactionId = transactionId;
        Amount = amount;
        Date = date;
        CategoryId = categoryId;
        Memo = memo;
        AffectsBudget = affectsBudget;
    }

    public Guid TransactionId { get; }
    public decimal Amount { get; }
    public DateOnly Date { get; }
    public Guid? CategoryId { get; }
    public string? Memo { get; }
    public bool AffectsBudget { get; }
}

public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommandInput>
{
    public UpdateTransactionCommandValidator()
    {
        RuleFor(x => x.Amount).NotEqual(0).WithMessage("Amount cannot be zero.");
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Memo).MaximumLength(500).When(x => x.Memo is not null);
    }
}
