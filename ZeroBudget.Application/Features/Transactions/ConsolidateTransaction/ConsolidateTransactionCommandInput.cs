using MediatR;

namespace ZeroBudget.Application.Features.Transactions.ConsolidateTransaction;

public class ConsolidateTransactionCommandInput : IRequest<ConsolidateTransactionCommandOutput>
{
    public ConsolidateTransactionCommandInput(Guid transactionId, bool isConsolidated)
    {
        TransactionId = transactionId;
        IsConsolidated = isConsolidated;
    }

    public Guid TransactionId { get; }
    public bool IsConsolidated { get; }
}
