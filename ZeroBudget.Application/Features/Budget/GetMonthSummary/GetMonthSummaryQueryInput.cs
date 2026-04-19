using MediatR;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Application.Features.Budget.GetMonthSummary;

public class GetMonthSummaryQueryInput : IRequest<GetMonthSummaryQueryOutput>
{
    public GetMonthSummaryQueryInput(YearMonth month)
    {
        Month = month;
    }

    public YearMonth Month { get; }
}
