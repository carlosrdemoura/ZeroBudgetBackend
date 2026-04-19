namespace ZeroBudget.Domain.Exceptions;

public class AccountHasTransactionsException()
    : DomainException("Account cannot be deleted because it has transactions.");
