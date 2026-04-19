using MediatR;

namespace ZeroBudget.Application.Features.Transactions.DeleteTransaction;

public class DeleteTransactionCommandInput : IRequest<Unit>
{
    public DeleteTransactionCommandInput(Guid transactionId)
    {
        TransactionId = transactionId;
    }

    public Guid TransactionId { get; }
}
