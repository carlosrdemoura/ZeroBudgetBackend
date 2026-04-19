using MediatR;

namespace ZeroBudget.Application.Features.Accounts.GetAccountsSummary;

public class GetAccountsSummaryQueryInput : IRequest<GetAccountsSummaryQueryOutput>;

public class GetAccountsSummaryQueryOutput(decimal totalBalance, decimal totalAssigned, decimal readyToAssign)
{
    public decimal TotalBalance { get; } = totalBalance;
    public decimal TotalAssigned { get; } = totalAssigned;
    public decimal ReadyToAssign { get; } = readyToAssign;
}
