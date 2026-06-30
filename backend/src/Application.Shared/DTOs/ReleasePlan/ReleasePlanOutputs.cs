namespace Application.Shared.DTOs.ReleasePlan
{
    public record ReleasePlanOutput
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public int ReleaseCount { get; init; }
        public string CreatedBy { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    public record ReleasePlanDetailOutput
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public List<ReleaseOutput> Releases { get; init; } = new();
        public string CreatedBy { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    public record ReleaseOutput
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime? StartDate { get; init; }
        public DateTime? EndDate { get; init; }
        public int StageCount { get; init; }
    }

    public record ReleaseDetailOutput : ReleaseOutput
    {
        public List<ReleaseStageOutput> Stages { get; init; } = new();
    }
}
