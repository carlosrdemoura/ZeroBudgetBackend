namespace ZeroBudget.Application.DTOs;

public class CategoryBalanceDTO
{
    public CategoryBalanceDTO(
        Guid categoryId,
        string categoryName,
        Guid groupId,
        string groupName,
        decimal previousBalance,
        decimal assigned,
        decimal activity,
        decimal balance)
    {
        CategoryId = categoryId;
        CategoryName = categoryName;
        GroupId = groupId;
        GroupName = groupName;
        PreviousBalance = previousBalance;
        Assigned = assigned;
        Activity = activity;
        Balance = balance;
    }

    public Guid CategoryId { get; }
    public string CategoryName { get; }
    public Guid GroupId { get; }
    public string GroupName { get; }
    public decimal PreviousBalance { get; }
    public decimal Assigned { get; }
    public decimal Activity { get; }
    public decimal Balance { get; }
}
