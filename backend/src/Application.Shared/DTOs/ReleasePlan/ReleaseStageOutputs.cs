namespace Application.Shared.DTOs.ReleasePlan
{
    public record ReleaseStageOutput
    {
        public int Id { get; init; }
        public int ReleaseId { get; init; }
        public int StageId { get; init; }
        public string? StageName { get; init; }
        public int Sequence { get; init; }
        public int WorkingDays { get; init; }
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public int StatusId { get; init; }
    }

    public record ReleaseStageHistoryOutput
    {
        public int Id { get; init; }
        public int ReleaseStageId { get; init; }
        public string ChangeType { get; init; } = string.Empty;
        public int? OldStatusId { get; init; }
        public string? OldStatus { get; init; }
        public int? NewStatusId { get; init; }
        public string? NewStatus { get; init; }
        public DateTime? OldStartDate { get; init; }
        public DateTime? NewStartDate { get; init; }
        public DateTime? OldEndDate { get; init; }
        public DateTime? NewEndDate { get; init; }
        public string? Notes { get; init; }
        public DateTime ChangedAt { get; init; }
        public string ChangedBy { get; init; } = string.Empty;
    }
}
