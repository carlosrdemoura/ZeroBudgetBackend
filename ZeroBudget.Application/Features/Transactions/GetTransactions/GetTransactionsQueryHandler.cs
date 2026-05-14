using MediatR;
using ZeroBudget.Application.Common;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;

namespace ZeroBudget.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQueryHandler(
    ITransactionRepository transactionRepository) : IRequestHandler<GetTransactionsQueryInput, GetTransactionsQueryOutput>
{
    public async Task<GetTransactionsQueryOutput> Handle(GetTransactionsQueryInput query, CancellationToken cancellationToken)
    {
        var paged = await transactionRepository.GetPagedAsync(
            query.Year,
            query.Month,
            query.Search,
            query.Page,
            query.PageSize,
            cancellationToken);

        var items = paged.Items.Select(t => new TransactionDTO(
            t.Id,
            t.Amount,
            t.Date,
            t.Description,
            t.IsConsolidated,
            t.Position,
            t.CreatedAt)).ToList();

        return new GetTransactionsQueryOutput(new PagedResult<TransactionDTO>(items, paged.TotalCount, paged.Page, paged.PageSize));
    }
}
