using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTransactionCommandInput, UpdateTransactionCommandOutput>
{
    public async Task<UpdateTransactionCommandOutput> Handle(UpdateTransactionCommandInput command, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(command.TransactionId, cancellationToken);
        if (transaction is null)
            throw new NotFoundException("Transaction", command.TransactionId);

        transaction.Update(command.Amount, command.Date, command.Description, command.IsConsolidated);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateTransactionCommandOutput(new TransactionDTO(
            transaction.Id,
            transaction.Amount,
            transaction.Date,
            transaction.Description,
            transaction.IsConsolidated,
            transaction.Position,
            transaction.CreatedAt));
    }
}
