namespace ZeroBudget.Application.Interfaces;

public interface IJwtTokenService
{
    (string Token, DateTime ExpiresAt) Generate(string email);
}
