using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Budget.GetMonthSummary;

public class GetMonthSummaryQueryOutput
{
    public GetMonthSummaryQueryOutput(MonthSummaryDTO summary)
    {
        Summary = summary;
    }

    public MonthSummaryDTO Summary { get; }
}
