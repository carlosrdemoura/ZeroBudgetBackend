using FluentAssertions;
using NSubstitute;
using ZeroBudget.Application.Features.Budget.GetMonthSummary;
using ZeroBudget.Application.Interfaces;
using ZeroBudget.Domain;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.UnitTests.Features.Budget;

public class GetMonthSummaryQueryHandlerTests
{
    private readonly ITransactionRepository _transactions = Substitute.For<ITransactionRepository>();
    private readonly IBudgetEntryRepository _budgetEntries = Substitute.For<IBudgetEntryRepository>();
    private readonly GetMonthSummaryQueryHandler _sut;

    private static readonly YearMonth Month = new(2025, 6);

    public GetMonthSummaryQueryHandlerTests()
    {
        _sut = new GetMonthSummaryQueryHandler(_transactions, _budgetEntries);

        _transactions.GetMonthlyIncomeAsync(Month, WellKnownIds.InflowCategoryId).Returns(0m);
        _budgetEntries.GetTotalAssignedForMonthAsync(Month).Returns(0m);
        _transactions.GetCumulativeIncomeAsync(Month.Previous(), WellKnownIds.InflowCategoryId).Returns(0m);
        _budgetEntries.GetCumulativeAssignedPerCategoryAsync(Month.Previous()).Returns(new Dictionary<Guid, decimal>());
    }

    [Fact]
    public async Task Handle_NoData_ReturnsZeroSummary()
    {
        var result = await _sut.Handle(new GetMonthSummaryQueryInput(Month), default);

        result.Summary.TotalIncome.Should().Be(0);
        result.Summary.TotalAssigned.Should().Be(0);
        result.Summary.AvailableToAssign.Should().Be(0);
        result.Summary.Rollover.Should().Be(0);
    }

    [Fact]
    public async Task Handle_IncomeNoAssignment_AvailableEqualsIncome()
    {
        _transactions.GetMonthlyIncomeAsync(Month, WellKnownIds.InflowCategoryId).Returns(3000m);

        var result = await _sut.Handle(new GetMonthSummaryQueryInput(Month), default);

        result.Summary.TotalIncome.Should().Be(3000m);
        result.Summary.AvailableToAssign.Should().Be(3000m);
    }

    [Fact]
    public async Task Handle_FullyAssigned_AvailableIsZero()
    {
        _transactions.GetMonthlyIncomeAsync(Month, WellKnownIds.InflowCategoryId).Returns(3000m);
        _budgetEntries.GetTotalAssignedForMonthAsync(Month).Returns(3000m);

        var result = await _sut.Handle(new GetMonthSummaryQueryInput(Month), default);

        result.Summary.AvailableToAssign.Should().Be(0);
    }

    [Fact]
    public async Task Handle_UnassignedIncomeFromPrevMonth_RollsIntoAvailable()
    {
        _transactions.GetCumulativeIncomeAsync(Month.Previous(), WellKnownIds.InflowCategoryId).Returns(1000m);
        _budgetEntries.GetCumulativeAssignedPerCategoryAsync(Month.Previous())
            .Returns(new Dictionary<Guid, decimal> { [Guid.NewGuid()] = 300m });

        var result = await _sut.Handle(new GetMonthSummaryQueryInput(Month), default);

        result.Summary.Rollover.Should().Be(700m);
        result.Summary.AvailableToAssign.Should().Be(700m);
    }

    [Fact]
    public async Task Handle_PrevMonthSpendingDoesNotReduceRollover()
    {
        var catId = Guid.NewGuid();
        _transactions.GetCumulativeIncomeAsync(Month.Previous(), WellKnownIds.InflowCategoryId).Returns(1000m);
        _budgetEntries.GetCumulativeAssignedPerCategoryAsync(Month.Previous())
            .Returns(new Dictionary<Guid, decimal> { [catId] = 500m });

        var result = await _sut.Handle(new GetMonthSummaryQueryInput(Month), default);

        result.Summary.Rollover.Should().Be(500m);
        result.Summary.AvailableToAssign.Should().Be(500m);
    }

    [Fact]
    public async Task Handle_OverAssignedPrevMonth_NegativeRollover()
    {
        var catId = Guid.NewGuid();
        _transactions.GetCumulativeIncomeAsync(Month.Previous(), WellKnownIds.InflowCategoryId).Returns(500m);
        _budgetEntries.GetCumulativeAssignedPerCategoryAsync(Month.Previous())
            .Returns(new Dictionary<Guid, decimal> { [catId] = 700m });
        _transactions.GetMonthlyIncomeAsync(Month, WellKnownIds.InflowCategoryId).Returns(1000m);

        var result = await _sut.Handle(new GetMonthSummaryQueryInput(Month), default);

        result.Summary.Rollover.Should().Be(-200m);
        result.Summary.AvailableToAssign.Should().Be(800m);
    }
}
