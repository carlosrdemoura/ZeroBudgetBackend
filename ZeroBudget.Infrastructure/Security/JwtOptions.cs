namespace ZeroBudget.Infrastructure.Security;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Secret { get; set; } = string.Empty;
    public int ExpirationDays { get; set; } = 7;
    public string Issuer { get; set; } = "ZeroBudget";
    public string Audience { get; set; } = "ZeroBudget";
}
