using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Lookups
{
    public static class DurationUnitsLookup
    {
        public static readonly DurationUnitsInfo Days = new(1, "Days");
        public static readonly DurationUnitsInfo Weeks = new(2, "Weeks");
        public static readonly DurationUnitsInfo Months = new(3, "Months");

        public static IEnumerable<DurationUnitsInfo> All =>
        [
            Days, Weeks, Months
        ];
    }

    public record DurationUnitsInfo(int Id, string Name);
}
