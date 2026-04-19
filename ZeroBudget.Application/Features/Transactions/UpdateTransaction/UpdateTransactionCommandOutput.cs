using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Transactions.UpdateTransaction;

public class UpdateTransactionCommandOutput
{
    public UpdateTransactionCommandOutput(TransactionDTO transaction)
    {
        Transaction = transaction;
    }

    public TransactionDTO Transaction { get; }
}
