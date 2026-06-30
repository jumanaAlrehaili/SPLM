namespace Domain.Lookups
{
    public static class StageLookup
    {
        public static readonly StageInfo BA = new(1, "BA Stage");
        public static readonly StageInfo SA = new(2, "SA Stage");
        public static readonly StageInfo UIUX = new(3, "UX/UI Stage");
        public static readonly StageInfo Dev = new(4, "DEV Stage");
        public static readonly StageInfo QA = new(5, "QA Stage");
        public static readonly StageInfo UAT = new(6, "UAT Stage");

        public static IEnumerable<StageInfo> All =>
        [
            BA, SA, UIUX, Dev, QA, UAT
        ];
    }

    public record StageInfo(int Id, string Name);
}
