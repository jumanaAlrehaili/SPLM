using Domain.Entities.Features;
using Domain.IdentityEntities;

namespace Domain.Entities.Releases
{
    public class ReleaseStageHistory
    {
        public int Id { get; set; }

        public int ReleaseStageId { get; set; }
        public ReleaseStage ReleaseStage { get; set; } = null!;

        public int? OldStatusId { get; set; }
        public ReleaseStageStatus? OldStatus { get; set; }

        public int? NewStatusId { get; set; }
        public ReleaseStageStatus? NewStatus { get; set; }

        public DateTime? OldStartDate { get; set; }
        public DateTime? NewStartDate { get; set; }

        public DateTime? OldEndDate { get; set; }
        public DateTime? NewEndDate { get; set; }

        public int ChangeType { get; set; }
        public string? Notes { get; set; }

        public DateTime ChangedAt { get; set; }
        public int ChangedByUserId { get; set; }
        public ApplicationUser ChangedByUser { get; set; } = null!;
    }
}
