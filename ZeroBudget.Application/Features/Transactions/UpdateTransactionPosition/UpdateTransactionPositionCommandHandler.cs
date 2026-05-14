using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Transactions.UpdateTransactionPosition;

public class UpdateTransactionPositionCommandHandler(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateTransactionPositionCommandInput, UpdateTransactionPositionCommandOutput>
{
    public async Task<UpdateTransactionPositionCommandOutput> Handle(UpdateTransactionPositionCommandInput command, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(command.TransactionId, cancellationToken);
        if (transaction is null)
            throw new NotFoundException("Transaction", command.TransactionId);

        transaction.SetPosition(command.Position);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateTransactionPositionCommandOutput();
    }
}
