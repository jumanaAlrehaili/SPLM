namespace Domain.Lookups
{
    public static class ReleaseStageStatusLookup
    {
        public static readonly ReleaseStageStatusInfo NotStarted = new(1, "Not Started");
        public static readonly ReleaseStageStatusInfo InProgress = new(2, "In Progress");
        public static readonly ReleaseStageStatusInfo Completed = new(3, "Completed");

        public static IEnumerable<ReleaseStageStatusInfo> All =>
        [
            NotStarted, InProgress, Completed
        ];
    }

    public record ReleaseStageStatusInfo(int Id, string Name);
}
