using MediatR;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.Application.Features.Categories.GetCategoryBalances;

public class GetCategoryBalancesQueryInput : IRequest<GetCategoryBalancesQueryOutput>
{
    public GetCategoryBalancesQueryInput(YearMonth month)
    {
        Month = month;
    }

    public YearMonth Month { get; }
}
