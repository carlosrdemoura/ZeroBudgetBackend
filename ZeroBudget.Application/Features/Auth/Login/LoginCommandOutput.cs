namespace ZeroBudget.Application.Features.Auth.Login;

public class LoginCommandOutput
{
    public LoginCommandOutput(string token, DateTime expiresAt, string email)
    {
        Token = token;
        ExpiresAt = expiresAt;
        Email = email;
    }

    public string Token { get; }
    public DateTime ExpiresAt { get; }
    public string Email { get; }
}
