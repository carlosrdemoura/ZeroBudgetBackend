namespace ZeroBudget.Domain.Exceptions;

public class AccountHasTransactionsException()
    : DomainException("Account cannot be deleted while its balance is non-zero. Transfer or adjust the balance to zero first.");
