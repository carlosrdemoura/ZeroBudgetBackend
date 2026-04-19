using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Accounts.DeleteAccount;

public class DeleteAccountCommandHandler(
    IAccountRepository accountRepository,
    ITransactionRepository transactionRepository,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteAccountCommandInput>
{
    public async Task Handle(DeleteAccountCommandInput command, CancellationToken cancellationToken)
    {
        var account = await accountRepository.GetByIdAsync(command.AccountId, cancellationToken);
        if (account is null)
            throw new NotFoundException("Account", command.AccountId);

        if (await accountRepository.HasExpenseTransactionsAsync(command.AccountId, cancellationToken))
            throw new AccountHasTransactionsException();

        await transactionRepository.DeleteByAccountAsync(command.AccountId, cancellationToken);
        await accountRepository.DeleteAsync(account, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
