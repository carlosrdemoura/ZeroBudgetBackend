using FluentAssertions;
using ZeroBudget.Domain.ValueObjects;

namespace ZeroBudget.UnitTests.ValueObjects;

public class YearMonthTests
{
    [Theory]
    [InlineData("2025-01", 2025, 1)]
    [InlineData("2025-12", 2025, 12)]
    [InlineData("2000-06", 2000, 6)]
    public void Parse_ValidString_ReturnsCorrectYearMonth(string input, int year, int month)
    {
        var result = YearMonth.Parse(input);

        result.Year.Should().Be(year);
        result.Month.Should().Be(month);
    }

    [Theory]
    [InlineData("")]
    [InlineData("2025")]
    [InlineData("2025-1")]
    [InlineData("25-01")]
    [InlineData("abcd-ef")]
    [InlineData(null)]
    public void Parse_InvalidString_Throws(string? input)
    {
        var act = () => YearMonth.Parse(input!);
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void ToString_ReturnsFormattedString()
    {
        var ym = new YearMonth(2025, 3);
        ym.ToString().Should().Be("2025-03");
    }

    [Fact]
    public void Next_MidYear_IncrementsMonth()
    {
        var ym = new YearMonth(2025, 6);
        ym.Next().Should().Be(new YearMonth(2025, 7));
    }

    [Fact]
    public void Next_December_WrapsToJanuaryNextYear()
    {
        var ym = new YearMonth(2025, 12);
        ym.Next().Should().Be(new YearMonth(2026, 1));
    }

    [Fact]
    public void Previous_MidYear_DecrementsMonth()
    {
        var ym = new YearMonth(2025, 6);
        ym.Previous().Should().Be(new YearMonth(2025, 5));
    }

    [Fact]
    public void Previous_January_WrapsToDecemberPreviousYear()
    {
        var ym = new YearMonth(2025, 1);
        ym.Previous().Should().Be(new YearMonth(2024, 12));
    }

    [Fact]
    public void IsBefore_EarlierMonth_ReturnsTrue()
    {
        var jan = new YearMonth(2025, 1);
        var feb = new YearMonth(2025, 2);
        jan.IsBefore(feb).Should().BeTrue();
    }

    [Fact]
    public void IsBefore_SameMonth_ReturnsFalse()
    {
        var ym = new YearMonth(2025, 1);
        ym.IsBefore(ym).Should().BeFalse();
    }

    [Fact]
    public void IsAfter_LaterMonth_ReturnsTrue()
    {
        var feb = new YearMonth(2025, 2);
        var jan = new YearMonth(2025, 1);
        feb.IsAfter(jan).Should().BeTrue();
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var a = new YearMonth(2025, 6);
        var b = new YearMonth(2025, 6);
        a.Should().Be(b);
    }

    [Fact]
    public void FirstDay_ReturnsFirstDayOfMonth()
    {
        var ym = new YearMonth(2025, 6);
        ym.FirstDay().Should().Be(new DateOnly(2025, 6, 1));
    }

    [Fact]
    public void FirstDayOfNext_MidYear_ReturnsFirstDayOfNextMonth()
    {
        var ym = new YearMonth(2025, 6);
        ym.FirstDayOfNext().Should().Be(new DateOnly(2025, 7, 1));
    }

    [Fact]
    public void FirstDayOfNext_December_WrapsToJanuaryNextYear()
    {
        var ym = new YearMonth(2025, 12);
        ym.FirstDayOfNext().Should().Be(new DateOnly(2026, 1, 1));
    }
}
