namespace ZeroBudget.Domain.Exceptions;

public class UnauthorizedException : DomainException
{
    public UnauthorizedException() : base("Invalid credentials.") { }
}
