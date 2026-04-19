using ZeroBudget.Application.Interfaces;

namespace ZeroBudget.Infrastructure.Security;

internal sealed class AuthSettings(string email, string password) : IAuthSettings
{
    public string Email { get; } = email;
    public string Password { get; } = password;
}
