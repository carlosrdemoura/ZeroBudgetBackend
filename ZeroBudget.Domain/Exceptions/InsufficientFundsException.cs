namespace ZeroBudget.Domain.Exceptions;

public class InsufficientFundsException : DomainException
{
    public InsufficientFundsException(decimal available, decimal requested)
        : base($"Cannot assign {requested:C}. Only {available:C} is available to assign.") { }
}
