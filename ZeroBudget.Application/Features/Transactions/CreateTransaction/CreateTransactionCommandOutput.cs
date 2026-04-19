using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Transactions.CreateTransaction;

public class CreateTransactionCommandOutput
{
    public CreateTransactionCommandOutput(TransactionDTO transaction)
    {
        Transaction = transaction;
    }

    public TransactionDTO Transaction { get; }
}
