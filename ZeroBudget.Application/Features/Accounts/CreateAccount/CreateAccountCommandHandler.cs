using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.Application.Features.Accounts.CreateAccount;

public class CreateAccountCommandHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateAccountCommandInput, CreateAccountCommandOutput>
{
    public async Task<CreateAccountCommandOutput> Handle(CreateAccountCommandInput command, CancellationToken cancellationToken)
    {
        var account = Account.Create(command.Name);

        var initialTx = Transaction.CreateInitialBalance(
            account.Id,
            command.InitialBalance,
            DateOnly.FromDateTime(DateTime.UtcNow),
            WellKnownIds.InflowCategoryId);

        await accountRepository.AddAsync(account, cancellationToken);
        await transactionRepository.AddAsync(initialTx, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateAccountCommandOutput(new AccountDTO(account.Id, account.Name, command.InitialBalance));
    }
}
