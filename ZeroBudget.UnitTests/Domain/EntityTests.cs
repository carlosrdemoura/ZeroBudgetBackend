using FluentAssertions;
using ZeroBudget.Domain.Entities;

namespace ZeroBudget.UnitTests.Domain;

public class EntityTests
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    [Fact]
    public void Transaction_Create_ZeroAmount_Throws()
    {
        var act = () => Transaction.Create(0m, Today, null, false);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Transaction_Create_NegativeAmount_Succeeds()
    {
        var t = Transaction.Create(-50m, Today, "Rent", false);
        t.Amount.Should().Be(-50m);
        t.Description.Should().Be("Rent");
        t.IsConsolidated.Should().BeFalse();
    }

    [Fact]
    public void Transaction_Create_PositiveAmount_Succeeds()
    {
        var t = Transaction.Create(1000m, Today, "Salary", true);
        t.Amount.Should().Be(1000m);
        t.IsConsolidated.Should().BeTrue();
    }

    [Fact]
    public void Transaction_Create_TrimsDescription()
    {
        var t = Transaction.Create(10m, Today, "  Coffee  ", false);
        t.Description.Should().Be("Coffee");
    }

    [Fact]
    public void Transaction_Update_ChangesAllFields()
    {
        var t = Transaction.Create(10m, Today, "Old", false);
        var newDate = Today.AddDays(1);

        t.Update(-25m, newDate, "New", true);

        t.Amount.Should().Be(-25m);
        t.Date.Should().Be(newDate);
        t.Description.Should().Be("New");
        t.IsConsolidated.Should().BeTrue();
    }

    [Fact]
    public void Transaction_Update_ZeroAmount_Throws()
    {
        var t = Transaction.Create(10m, Today, null, false);
        var act = () => t.Update(0m, Today, null, false);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Transaction_SetConsolidated_TogglesFlag()
    {
        var t = Transaction.Create(10m, Today, null, false);
        t.SetConsolidated(true);
        t.IsConsolidated.Should().BeTrue();
        t.SetConsolidated(false);
        t.IsConsolidated.Should().BeFalse();
    }
}
