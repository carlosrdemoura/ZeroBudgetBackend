using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Transactions.UpdateTransactionPosition;

public class UpdateTransactionPositionCommandInput : IRequest<UpdateTransactionPositionCommandOutput>
{
    public UpdateTransactionPositionCommandInput(Guid transactionId, double position)
    {
        TransactionId = transactionId;
        Position = position;
    }

    public Guid TransactionId { get; }
    public double Position { get; }
}

public class UpdateTransactionPositionCommandValidator : AbstractValidator<UpdateTransactionPositionCommandInput>
{
    public UpdateTransactionPositionCommandValidator()
    {
        RuleFor(x => x.Position)
            .Must(p => !double.IsNaN(p) && !double.IsInfinity(p))
            .WithMessage("Position must be a finite number.");
    }
}
