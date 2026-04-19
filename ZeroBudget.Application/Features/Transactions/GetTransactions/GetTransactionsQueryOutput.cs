using ZeroBudget.Application.Common;
using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQueryOutput
{
    public GetTransactionsQueryOutput(PagedResult<TransactionDTO> transactions)
    {
        Transactions = transactions;
    }

    public PagedResult<TransactionDTO> Transactions { get; }
}
