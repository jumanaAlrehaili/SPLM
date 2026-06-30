using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Lookups
{
    public static class FeaturePriorityLookup
    {
        public static readonly FeaturePriorityInfo Low = new(1, "Low");
        public static readonly FeaturePriorityInfo Medium = new(2, "Medium");
        public static readonly FeaturePriorityInfo High = new(3, "High"); 
        public static readonly FeaturePriorityInfo Critical = new(4, "Critical");

        public static IEnumerable<FeaturePriorityInfo> All =>      
        [
            Low, Medium, High, Critical
        ];
    }

    public record FeaturePriorityInfo(int Id, string Name);
}
