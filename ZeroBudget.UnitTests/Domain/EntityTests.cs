using FluentAssertions;
using ZeroBudget.Domain.Entities;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.UnitTests.Domain;

public class EntityTests
{
    // ── Category ─────────────────────────────────────────────────────────────

    [Fact]
    public void Category_Rename_TrimsWhitespace()
    {
        var cat = Category.Create(Guid.NewGuid(), "Food");
        cat.Rename("  Groceries  ");
        cat.Name.Should().Be("Groceries");
    }

    // ── BudgetEntry ───────────────────────────────────────────────────────────

    [Fact]
    public void BudgetEntry_Create_NegativeAssigned_Succeeds()
    {
        var entry = BudgetEntry.Create(Guid.NewGuid(), new YearMonth(2025, 1), -50m);
        entry.Assigned.Should().Be(-50m);
    }

    [Fact]
    public void BudgetEntry_UpdateAssigned_NegativeAmount_Succeeds()
    {
        var entry = BudgetEntry.Create(Guid.NewGuid(), new YearMonth(2025, 1), 100m);
        entry.UpdateAssigned(-25m);
        entry.Assigned.Should().Be(-25m);
    }

    [Fact]
    public void BudgetEntry_UpdateAssigned_ValidAmount_UpdatesValue()
    {
        var entry = BudgetEntry.Create(Guid.NewGuid(), new YearMonth(2025, 1), 100m);
        entry.UpdateAssigned(250m);
        entry.Assigned.Should().Be(250m);
    }

    // ── Transaction ───────────────────────────────────────────────────────────

    private static readonly Guid AccountId = Guid.NewGuid();
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    [Fact]
    public void Transaction_Create_ZeroAmount_Throws()
    {
        var act = () => Transaction.Create(AccountId, 0m, Today, null);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Transaction_Create_NegativeAmount_NoCategory_Succeeds()
    {
        var t = Transaction.Create(AccountId, -50m, Today, null);
        t.Amount.Should().Be(-50m);
        t.CategoryId.Should().BeNull();
    }

    [Fact]
    public void Transaction_Create_NegativeAmount_WithCategory_Succeeds()
    {
        var categoryId = Guid.NewGuid();
        var t = Transaction.Create(AccountId, -50m, Today, categoryId);
        t.Amount.Should().Be(-50m);
        t.CategoryId.Should().Be(categoryId);
    }

    [Fact]
    public void Transaction_Create_PositiveAmount_NoCategory_Succeeds()
    {
        var t = Transaction.Create(AccountId, 1000m, Today, null);
        t.Amount.Should().Be(1000m);
        t.CategoryId.Should().BeNull();
    }

    [Fact]
    public void Transaction_Create_DefaultAffectsBudget_IsTrue()
    {
        var t = Transaction.Create(AccountId, 100m, Today, null);
        t.AffectsBudget.Should().BeTrue();
    }

    [Fact]
    public void Transaction_Create_AffectsBudgetFalse_IsFalse()
    {
        var t = Transaction.Create(AccountId, 100m, Today, null, memo: null, affectsBudget: false);
        t.AffectsBudget.Should().BeFalse();
    }
}
