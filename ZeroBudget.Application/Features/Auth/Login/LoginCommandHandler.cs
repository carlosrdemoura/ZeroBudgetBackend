using MediatR;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain.Exceptions;

namespace ZeroBudget.Application.Features.Auth.Login;

public class LoginCommandHandler(
    IAuthSettings authSettings,
    IJwtTokenService jwtTokenService) : IRequestHandler<LoginCommandInput, LoginCommandOutput>
{
    public Task<LoginCommandOutput> Handle(LoginCommandInput command, CancellationToken cancellationToken)
    {
        var emailMatch = command.Email.Equals(authSettings.Email, StringComparison.OrdinalIgnoreCase);
        if (!emailMatch || command.Password != authSettings.Password)
            throw new UnauthorizedException();

        var (token, expiresAt) = jwtTokenService.Generate(authSettings.Email);

        return Task.FromResult(new LoginCommandOutput(token, expiresAt, authSettings.Email));
    }
}
