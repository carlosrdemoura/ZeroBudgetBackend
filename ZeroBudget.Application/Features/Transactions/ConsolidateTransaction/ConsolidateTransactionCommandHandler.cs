using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Transactions.ConsolidateTransaction;

public class ConsolidateTransactionCommandHandler(
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<ConsolidateTransactionCommandInput, ConsolidateTransactionCommandOutput>
{
    public async Task<ConsolidateTransactionCommandOutput> Handle(ConsolidateTransactionCommandInput command, CancellationToken cancellationToken)
    {
        var transaction = await transactionRepository.GetByIdAsync(command.TransactionId, cancellationToken);
        if (transaction is null)
            throw new NotFoundException("Transaction", command.TransactionId);

        transaction.SetConsolidated(command.IsConsolidated);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new ConsolidateTransactionCommandOutput(new TransactionDTO(
            transaction.Id,
            transaction.Amount,
            transaction.Date,
            transaction.Description,
            transaction.IsConsolidated,
            transaction.CreatedAt));
    }
}
