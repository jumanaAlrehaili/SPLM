namespace Domain.Lookups
{
    public static class FeatureStatusLookup
    {
        public static readonly FeatureStatusInfo New = new(1, "New");
        public static readonly FeatureStatusInfo InProgress = new(2, "In Progress");
        public static readonly FeatureStatusInfo PendingReview = new(3, "Pending Review");
        public static readonly FeatureStatusInfo Rejected = new(4, "Rejected");
        public static readonly FeatureStatusInfo Completed = new(5, "Completed");

        public static IEnumerable<FeatureStatusInfo> All =>
        [
            New, InProgress, PendingReview, Rejected, Completed
        ];
    }

    public record FeatureStatusInfo(int Id, string Name);
}
