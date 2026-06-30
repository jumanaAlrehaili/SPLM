namespace Application.Common;

public static class WorkingDaysCalculator
{
    public static DateTime Calculate(DateTime startDate, int workingDays, IEnumerable<(DateOnly Start, DateOnly End)> holidayRanges)
    {
        if (workingDays < 1)
            throw new ArgumentException("Working days must be at least 1.", nameof(workingDays));

        var ranges = holidayRanges.ToList();
        var counted = 0;
        var current = DateOnly.FromDateTime(startDate);

        while (true)
        {
            if (IsWorkingDay(current, ranges))
            {
                counted++;
                if (counted == workingDays)
                    return current.ToDateTime(TimeOnly.MinValue);
            }

            current = current.AddDays(1);
        }
    }

    private static bool IsWorkingDay(DateOnly date, List<(DateOnly Start, DateOnly End)> holidayRanges)
    {
        if (date.DayOfWeek == DayOfWeek.Friday || date.DayOfWeek == DayOfWeek.Saturday)
            return false;

        return !holidayRanges.Any(r => date >= r.Start && date <= r.End);
    }
}
