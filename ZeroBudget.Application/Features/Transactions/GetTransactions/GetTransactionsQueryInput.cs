using MediatR;

namespace ZeroBudget.Application.Features.Transactions.GetTransactions;

public class GetTransactionsQueryInput : IRequest<GetTransactionsQueryOutput>
{
    public GetTransactionsQueryInput(
        int year,
        int month,
        string? search = null,
        int page = 1,
        int pageSize = 50)
    {
        Year = year;
        Month = month;
        Search = search;
        Page = page;
        PageSize = pageSize;
    }

    public int Year { get; }
    public int Month { get; }
    public string? Search { get; }
    public int Page { get; }
    public int PageSize { get; }
}
