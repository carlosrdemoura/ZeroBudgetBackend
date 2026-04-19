namespace ZeroBudget.Domain.Services;

/// <summary>
/// Pure, stateless domain logic for budget calculations.
/// No dependencies on EF Core, repositories, or infrastructure.
/// </summary>
public static class BudgetCalculationService
{
    /// <summary>
    /// Category balance = assigned amount + net activity (sum of transactions).
    /// Activity is negative for expenses, positive for income assigned to a category.
    /// </summary>
    public static decimal CalculateCategoryBalance(decimal assigned, decimal activity)
        => assigned + activity;

    /// <summary>
    /// Available to assign = total income this month - total assigned across all categories.
    /// Must never go below zero (enforced by the application layer before committing).
    /// </summary>
    public static decimal CalculateAvailableToAssign(decimal totalIncome, decimal totalAssigned)
        => totalIncome - totalAssigned;

    /// <summary>
    /// Determines how much of a previous month's category balance rolls into the next month.
    /// Positive balance: carries forward as-is.
    /// Negative balance (overspending): returns 0; the debt is handled separately via CalculateRolloverDebt.
    /// </summary>
    public static decimal CalculateRolloverCarryOver(decimal previousBalance)
        => Math.Max(0, previousBalance);

    /// <summary>
    /// Sum of all overspent category balances from the previous month.
    /// This amount is subtracted from available-to-assign in the new month.
    /// </summary>
    public static decimal CalculateRolloverDebt(IEnumerable<decimal> previousCategoryBalances)
        => previousCategoryBalances.Where(b => b < 0).Sum();

    /// <summary>
    /// Returns true if assigning the new total would exceed available income.
    /// </summary>
    public static bool WouldExceedBudget(decimal totalIncome, decimal currentTotalAssigned, decimal newCategoryAssigned, decimal previousCategoryAssigned)
    {
        var newTotal = currentTotalAssigned - previousCategoryAssigned + newCategoryAssigned;
        return newTotal > totalIncome;
    }
}
