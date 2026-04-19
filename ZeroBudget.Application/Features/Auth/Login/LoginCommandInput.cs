using FluentValidation;
using MediatR;

namespace ZeroBudget.Application.Features.Auth.Login;

public class LoginCommandInput : IRequest<LoginCommandOutput>
{
    public LoginCommandInput(string email, string password)
    {
        Email = email;
        Password = password;
    }

    public string Email { get; }
    public string Password { get; }
}

public class LoginCommandValidator : AbstractValidator<LoginCommandInput>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}
