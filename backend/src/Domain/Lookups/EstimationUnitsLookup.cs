using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Lookups
{
    public static class EstimationUnitsLookup
    {
        public static readonly EstimationUnitsInfo Hours = new(1, "Hours");
        public static readonly EstimationUnitsInfo Days = new(2, "Days");

        public static IEnumerable<EstimationUnitsInfo> All =>
        [
           Hours, Days
        ];
    }

    public record EstimationUnitsInfo(int Id, string Name);
}
