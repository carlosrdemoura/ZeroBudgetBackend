namespace ZeroBudget.Application.DTOs;

public class MonthSummaryDTO
{
    public MonthSummaryDTO(
        string month,
        decimal totalIncome,
        decimal totalAssigned,
        decimal availableToAssign,
        decimal rollover)
    {
        Month = month;
        TotalIncome = totalIncome;
        TotalAssigned = totalAssigned;
        AvailableToAssign = availableToAssign;
        Rollover = rollover;
    }

    public string Month { get; }
    public decimal TotalIncome { get; }
    public decimal TotalAssigned { get; }
    public decimal AvailableToAssign { get; }
    public decimal Rollover { get; }
}
