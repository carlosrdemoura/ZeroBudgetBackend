using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;

namespace ZeroBudget.Application.Features.Accounts.GetAccounts;

public class GetAccountsQueryHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository) : IRequestHandler<GetAccountsQueryInput, GetAccountsQueryOutput>
{
    public async Task<GetAccountsQueryOutput> Handle(GetAccountsQueryInput query, CancellationToken cancellationToken)
    {
        var accounts = await accountRepository.GetAllAsync(cancellationToken);
        var balances = await transactionRepository.GetAccountBalancesAsync(cancellationToken);

        var dtos = accounts
            .Select(a => new AccountDTO(a.Id, a.Name, balances.GetValueOrDefault(a.Id, 0m)))
            .ToList();

        return new GetAccountsQueryOutput(dtos);
    }
}
