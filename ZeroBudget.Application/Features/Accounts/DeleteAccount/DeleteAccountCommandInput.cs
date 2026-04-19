using MediatR;

namespace ZeroBudget.Application.Features.Accounts.DeleteAccount;

public class DeleteAccountCommandInput : IRequest
{
    public DeleteAccountCommandInput(Guid accountId)
    {
        AccountId = accountId;
    }

    public Guid AccountId { get; }
}
