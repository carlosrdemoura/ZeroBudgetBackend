using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteTransactionCommandInput, Unit>
{
    public async Task<Unit> Handle(DeleteTransactionCommandInput command, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(command.TransactionId, cancellationToken);
        if (transaction is null)
            throw new NotFoundException("Transaction", command.TransactionId);

        await transactionRepository.DeleteAsync(transaction, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
