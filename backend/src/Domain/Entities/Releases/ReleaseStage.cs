using Domain.Common;
using Domain.Entities.Features;
using Domain.IdentityEntities;

namespace Domain.Entities.Releases
{
    public class ReleaseStage : AuditedEntity
    {
        public int Id { get; set; }

        public int ReleaseId { get; set; }
        public Release Release { get; set; } = null!;

        public int StageId { get; set; }
        public Stage Stage { get; set; } = null!;

        public string? StageName { get; set; }

        public int Sequence { get; set; }

        public int WorkingDays { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        // Set by the deadline-monitor job once leads have been warned that this stage's deadline is approaching.
        // Prevents the same stage from generating a "due soon" notification on every monitor tick.
        public DateTime? DueSoonNotifiedAt { get; set; }

        public int StatusId { get; set; }
        public ReleaseStageStatus Status { get; set; } = null!;

        public ICollection<ReleaseStagePrerequisite> Prerequisites { get; set; } = new HashSet<ReleaseStagePrerequisite>();
        public ICollection<ReleaseStageHistory> Histories { get; set; } = new HashSet<ReleaseStageHistory>();
    }
}
