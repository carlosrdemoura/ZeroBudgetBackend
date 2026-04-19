using FluentAssertions;
using ZeroBudget.Domain.Services;

namespace ZeroBudget.UnitTests.Domain;

public class BudgetCalculationServiceTests
{
    // ── CalculateCategoryBalance ──────────────────────────────────────────────

    [Fact]
    public void CalculateCategoryBalance_AssignedPlusActivity_ReturnsSum()
    {
        BudgetCalculationService.CalculateCategoryBalance(500m, -200m).Should().Be(300m);
    }

    [Fact]
    public void CalculateCategoryBalance_NoActivity_ReturnsAssigned()
    {
        BudgetCalculationService.CalculateCategoryBalance(500m, 0m).Should().Be(500m);
    }

    [Fact]
    public void CalculateCategoryBalance_ActivityExceedsAssigned_ReturnsNegative()
    {
        BudgetCalculationService.CalculateCategoryBalance(100m, -300m).Should().Be(-200m);
    }

    [Fact]
    public void CalculateCategoryBalance_ZeroAssignedAndActivity_ReturnsZero()
    {
        BudgetCalculationService.CalculateCategoryBalance(0m, 0m).Should().Be(0m);
    }

    // ── CalculateAvailableToAssign ────────────────────────────────────────────

    [Fact]
    public void CalculateAvailableToAssign_PartiallyAssigned_ReturnsRemainder()
    {
        BudgetCalculationService.CalculateAvailableToAssign(1000m, 600m).Should().Be(400m);
    }

    [Fact]
    public void CalculateAvailableToAssign_FullyAssigned_ReturnsZero()
    {
        BudgetCalculationService.CalculateAvailableToAssign(1000m, 1000m).Should().Be(0m);
    }

    [Fact]
    public void CalculateAvailableToAssign_NothingAssigned_ReturnsFullIncome()
    {
        BudgetCalculationService.CalculateAvailableToAssign(1000m, 0m).Should().Be(1000m);
    }

    [Fact]
    public void CalculateAvailableToAssign_ZeroIncome_ReturnsNegative()
    {
        // Application layer must reject this; domain just computes the value
        BudgetCalculationService.CalculateAvailableToAssign(0m, 200m).Should().Be(-200m);
    }

    // ── CalculateRolloverCarryOver ────────────────────────────────────────────

    [Fact]
    public void CalculateRolloverCarryOver_PositiveBalance_CarriesForward()
    {
        BudgetCalculationService.CalculateRolloverCarryOver(250m).Should().Be(250m);
    }

    [Fact]
    public void CalculateRolloverCarryOver_ZeroBalance_ReturnsZero()
    {
        BudgetCalculationService.CalculateRolloverCarryOver(0m).Should().Be(0m);
    }

    [Fact]
    public void CalculateRolloverCarryOver_NegativeBalance_ReturnsZero()
    {
        BudgetCalculationService.CalculateRolloverCarryOver(-150m).Should().Be(0m);
    }

    // ── CalculateRolloverDebt ─────────────────────────────────────────────────

    [Fact]
    public void CalculateRolloverDebt_SomeOverspent_SumsNegativeBalances()
    {
        var balances = new[] { 100m, -50m, 200m, -75m };
        BudgetCalculationService.CalculateRolloverDebt(balances).Should().Be(-125m);
    }

    [Fact]
    public void CalculateRolloverDebt_AllPositive_ReturnsZero()
    {
        var balances = new[] { 100m, 200m, 50m };
        BudgetCalculationService.CalculateRolloverDebt(balances).Should().Be(0m);
    }

    [Fact]
    public void CalculateRolloverDebt_Empty_ReturnsZero()
    {
        BudgetCalculationService.CalculateRolloverDebt([]).Should().Be(0m);
    }

    // ── WouldExceedBudget ─────────────────────────────────────────────────────

    [Fact]
    public void WouldExceedBudget_AssigningWithinLimit_ReturnsFalse()
    {
        // income=1000, total already assigned=600, category currently has 200, setting to 300
        BudgetCalculationService.WouldExceedBudget(1000m, 600m, 300m, 200m).Should().BeFalse();
    }

    [Fact]
    public void WouldExceedBudget_AssigningExactlyAtLimit_ReturnsFalse()
    {
        // income=1000, total=600, category=200 → new total = 600 - 200 + 400 = 1000
        BudgetCalculationService.WouldExceedBudget(1000m, 600m, 400m, 200m).Should().BeFalse();
    }

    [Fact]
    public void WouldExceedBudget_AssigningOverLimit_ReturnsTrue()
    {
        // income=1000, total=600, category=200 → new total = 600 - 200 + 700 = 1100 > 1000
        BudgetCalculationService.WouldExceedBudget(1000m, 600m, 700m, 200m).Should().BeTrue();
    }

    [Fact]
    public void WouldExceedBudget_ZeroIncome_AssigningAny_ReturnsTrue()
    {
        BudgetCalculationService.WouldExceedBudget(0m, 0m, 1m, 0m).Should().BeTrue();
    }
}
