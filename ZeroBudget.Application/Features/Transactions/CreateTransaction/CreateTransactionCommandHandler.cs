using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Application.Features.Transactions.CreateTransaction;

public class CreateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateTransactionCommandInput, CreateTransactionCommandOutput>
{
    public async Task<CreateTransactionCommandOutput> Handle(CreateTransactionCommandInput command, CancellationToken cancellationToken)
    {
        var transaction = Transaction.Create(command.Amount, command.Date, command.Description, command.IsConsolidated);

        await transactionRepository.AddAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateTransactionCommandOutput(new TransactionDTO(
            transaction.Id,
            transaction.Amount,
            transaction.Date,
            transaction.Description,
            transaction.IsConsolidated,
            transaction.CreatedAt));
    }
}
