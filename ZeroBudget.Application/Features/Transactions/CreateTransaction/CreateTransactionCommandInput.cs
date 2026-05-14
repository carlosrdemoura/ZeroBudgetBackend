using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Transactions.CreateTransaction;

public class CreateTransactionCommandInput : IRequest<CreateTransactionCommandOutput>
{
    public CreateTransactionCommandInput(decimal amount, DateOnly date, string? description, bool isConsolidated, double? position)
    {
        Amount = amount;
        Date = date;
        Description = description;
        IsConsolidated = isConsolidated;
        Position = position;
    }

    public decimal Amount { get; }
    public DateOnly Date { get; }
    public string? Description { get; }
    public bool IsConsolidated { get; }
    public double? Position { get; }
}

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommandInput>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Amount).NotEqual(0).WithMessage("Amount cannot be zero.");
        RuleFor(x => x.Date).NotEmpty();
        RuleFor(x => x.Description).MaximumLength(500).When(x => x.Description is not null);
        RuleFor(x => x.Position!.Value)
            .Must(p => !double.IsNaN(p) && !double.IsInfinity(p))
            .When(x => x.Position.HasValue)
            .WithMessage("Position must be a finite number.");
    }
}
