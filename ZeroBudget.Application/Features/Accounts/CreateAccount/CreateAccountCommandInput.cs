using FluentValidation;
using MediatR;
using ZeroBudget.Application.DTOs;

namespace ZeroBudget.Application.Features.Accounts.CreateAccount;

public class CreateAccountCommandInput : IRequest<CreateAccountCommandOutput>
{
    public CreateAccountCommandInput(string name, decimal initialBalance)
    {
        Name = name;
        InitialBalance = initialBalance;
    }

    public string Name { get; }
    public decimal InitialBalance { get; }
}

public class CreateAccountCommandOutput(AccountDTO account)
{
    public AccountDTO Account { get; } = account;
}

public class CreateAccountCommandValidator : AbstractValidator<CreateAccountCommandInput>
{
    public CreateAccountCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
