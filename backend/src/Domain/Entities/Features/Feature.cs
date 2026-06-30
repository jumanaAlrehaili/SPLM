using Domain.Common;
using Domain.Entities.Projects;
using Domain.Entities.Releases;

namespace Domain.Entities.Features
{
    public class Feature : AuditedEntity
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? EpicLink { get; set; }
        public int PriorityId { get; set; }
        public FeaturePriority Priority { get; set; } = null!;

        public DateTime? CompletedAt { get; set; }

        public int CurrentStatusId { get; set; }
        public Status CurrentStatus { get; set; } = null!;

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int? ReleaseId { get; set; }
        public Release? Release { get; set; }

        public ICollection<FeatureAssignment> Assignments { get; set; } = new List<FeatureAssignment>();
        public virtual ICollection<FeatureStageLog> FeatureStageLogs { get; set; } = new List<FeatureStageLog>();
    }
}
