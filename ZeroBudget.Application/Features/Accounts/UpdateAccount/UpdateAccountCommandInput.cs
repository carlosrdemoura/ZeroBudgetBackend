using FluentValidation;
using MediatR;
using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Accounts.UpdateAccount;

public class UpdateAccountCommandInput : IRequest<UpdateAccountCommandOutput>
{
    public UpdateAccountCommandInput(Guid accountId, string name, decimal currentBalance)
    {
        AccountId = accountId;
        Name = name;
        CurrentBalance = currentBalance;
    }

    public Guid AccountId { get; }
    public string Name { get; }
    public decimal CurrentBalance { get; }
}

public class UpdateAccountCommandOutput(AccountDTO account)
{
    public AccountDTO Account { get; } = account;
}

public class UpdateAccountCommandValidator : AbstractValidator<UpdateAccountCommandInput>
{
    public UpdateAccountCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
