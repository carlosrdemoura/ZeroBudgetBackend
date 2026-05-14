using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandInput : IRequest<UpdateTransactionCommandOutput>
{
    public UpdateTransactionCommandInput(Guid transactionId, decimal amount, DateOnly date, string? description, bool isConsolidated)
    {
        TransactionId = transactionId;
        Amount = amount;
        Date = date;
        Description = description;
        IsConsolidated = isConsolidated;
    }

    public Guid TransactionId { get; }
    public decimal Amount { get; }
    public DateOnly Date { get; }
    public string? Description { get; }
    public bool IsConsolidated { get; }
}

public class UpdateTransactionCommandValidator : AbstractValidator<UpdateTransactionCommandInput>
{
    public UpdateTransactionCommandValidator()
    {
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
    }
}
