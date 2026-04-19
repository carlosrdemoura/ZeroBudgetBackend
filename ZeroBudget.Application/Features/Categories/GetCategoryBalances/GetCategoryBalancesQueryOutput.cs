using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Categories.GetCategoryBalances;

public class GetCategoryBalancesQueryOutput
{
    public GetCategoryBalancesQueryOutput(IEnumerable<CategoryBalanceDTO> balances)
    {
        Balances = balances;
    }

    public IEnumerable<CategoryBalanceDTO> Balances { get; }
}
