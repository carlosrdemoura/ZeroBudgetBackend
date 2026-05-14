using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Transactions.ConsolidateTransaction;

public class ConsolidateTransactionCommandOutput
{
    public ConsolidateTransactionCommandOutput(TransactionDTO transaction)
    {
        Transaction = transaction;
    }

    public TransactionDTO Transaction { get; }
}
