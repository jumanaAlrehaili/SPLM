namespace Application.Shared.DTOs.Feature
{
    public record FeatureOutput
    {
        public int Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string? Description { get; init; }
        public string? EpicLink { get; init; }
        public int PriorityId { get; init; }
        public DateTime? CompletedAt { get; init; }
        public string Status { get; init; } = string.Empty;
        public FeatureReleaseDto? Release { get; init; }
        public string CreatedBy { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    public record FeatureDetailOutput : FeatureOutput 
    {
        public IEnumerable<FeatureAssignmentDto> Assignments { get; init; } = new List<FeatureAssignmentDto>();
        public FeaturePermissionOutput Permissions { get; set; }
    }

    public record FeatureAssignmentDto
    {
        public int StageId { get; init; }
        public string StageName { get; init; } = string.Empty;
        public int? UserId { get; init; }
        public string? UserName { get; init; } = string.Empty;

        public int? Estimation { get; set; }
        public string? EstimationUnitName { get; set; }

        public DateTime? StartedAt { get; set; }
        public DateTime? CompletedAt { get; init; }
    }

    public record AvailableStageResourceDto(int UserId, string FullName);

    public record FeatureReleaseDto
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
    }
}
