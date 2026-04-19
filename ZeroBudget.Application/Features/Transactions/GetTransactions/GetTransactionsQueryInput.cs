using MediatR;

namespace ZeroBudget.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQueryInput : IRequest<GetTransactionsQueryOutput>
{
    public GetTransactionsQueryInput(
        Guid accountId,
        DateOnly fromDate,
        DateOnly toDate,
        string? search = null,
        int page = 1,
        int pageSize = 50)
    {
        AccountId = accountId;
        FromDate = fromDate;
        ToDate = toDate;
        Search = search;
        Page = page;
        PageSize = pageSize;
    }

    public Guid AccountId { get; }
    public DateOnly FromDate { get; }
    public DateOnly ToDate { get; }
    public string? Search { get; }
    public int Page { get; }
    public int PageSize { get; }
}
