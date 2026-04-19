using MediatR;
using ZeroBudget.Application.DTOs;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Accounts.UpdateAccount;

public class UpdateAccountCommandHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateAccountCommandInput, UpdateAccountCommandOutput>
{
    public async Task<UpdateAccountCommandOutput> Handle(UpdateAccountCommandInput command, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(command.AccountId, cancellationToken);
        if (account is null)
            throw new NotFoundException("Account", command.AccountId);

        var balances = await transactionRepository.GetAccountBalancesAsync(cancellationToken);
        var currentBalance = balances.GetValueOrDefault(command.AccountId, 0m);

        if (account.Name != command.Name)
            account.Rename(command.Name);

        if (currentBalance != command.CurrentBalance)
        {
            var diff = command.CurrentBalance - currentBalance;
            var adjustment = Transaction.CreateAdjustment(
                command.AccountId,
                diff,
                DateOnly.FromDateTime(DateTime.UtcNow),
                WellKnownIds.InflowCategoryId);

            await transactionRepository.AddAsync(adjustment, cancellationToken);
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateAccountCommandOutput(new AccountDTO(account.Id, account.Name, command.CurrentBalance));
    }
}
