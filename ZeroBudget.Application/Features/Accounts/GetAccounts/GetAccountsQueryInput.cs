using MediatR;
using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Accounts.GetAccounts;

public class GetAccountsQueryInput : IRequest<GetAccountsQueryOutput>;

public class GetAccountsQueryOutput(List<AccountDTO> accounts)
{
    public List<AccountDTO> Accounts { get; } = accounts;
}
