using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Application.Features.Transactions.CreateTransaction;

public class CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTransactionCommandInput, CreateTransactionCommandOutput>
{
    private const double PositionStep = 1024.0;

    public async Task<CreateTransactionCommandOutput> Handle(CreateTransactionCommandInput command, CancellationToken cancellationToken)
    {
        double position;
        if (command.Position.HasValue)
        {
            position = command.Position.Value;
        }
        else
        {
            var maxPosition = await transactionRepository.GetMaxPositionAsync(cancellationToken);
            position = (maxPosition ?? 0.0) + PositionStep;
        }

        var transaction = Transaction.Create(command.Amount, command.Date, command.Description, command.IsConsolidated, position, command.Id);

        await transactionRepository.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateTransactionCommandOutput(new TransactionDTO(
            transaction.Id,
            transaction.Amount,
            transaction.Date,
            transaction.Description,
            transaction.IsConsolidated,
            transaction.Position,
            transaction.CreatedAt));
    }
}
