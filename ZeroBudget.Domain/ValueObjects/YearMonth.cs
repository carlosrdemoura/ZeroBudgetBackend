namespace ZeroBudget.Domain.ValueObjects;

public record YearMonth(int Year, int Month)
{
    public static YearMonth Parse(string s)
    {
        if (s is null || s.Length != 7 || s[4] != '-')
            throw new ArgumentException($"Invalid YearMonth format: '{s}'. Expected YYYY-MM.");

        if (!int.TryParse(s.AsSpan(0, 4), out var year) ||
            !int.TryParse(s.AsSpan(5, 2), out var month))
            throw new ArgumentException($"Invalid YearMonth format: '{s}'. Expected YYYY-MM.");

        return new YearMonth(year, month);
    }

    public static YearMonth Current() => FromDateTime(DateTime.UtcNow);

    public static YearMonth FromDateTime(DateTime dt) => new(dt.Year, dt.Month);

    public YearMonth Next()
    {
        return Month == 12
            ? new YearMonth(Year + 1, 1)
            : new YearMonth(Year, Month + 1);
    }

    public YearMonth Previous()
    {
        return Month == 1
            ? new YearMonth(Year - 1, 12)
            : new YearMonth(Year, Month - 1);
    }

    public bool IsBefore(YearMonth other) =>
        Year < other.Year || (Year == other.Year && Month < other.Month);

    public bool IsAfter(YearMonth other) =>
        Year > other.Year || (Year == other.Year && Month > other.Month);

    /// <summary>Returns the first day of this month, for use in date-range SQL queries.</summary>
    public DateOnly FirstDay() => new DateOnly(Year, Month, 1);

    /// <summary>Returns the first day of the next month (exclusive upper bound for range queries).</summary>
    public DateOnly FirstDayOfNext() => Next().FirstDay();

    public override string ToString() => $"{Year:D4}-{Month:D2}";
}
