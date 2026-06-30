using Domain.Common;
using Domain.IdentityEntities;

namespace Domain.Entities.Features
{
    public class FeatureAssignment : AuditedEntity
    {
        public int Id { get; set; }

        public int FeatureId { get; set; }
        public Feature Feature { get; set; } = null!;

        public int StageId { get; set; }
        public Stage Stage { get; set; } = null!;

        public int AssignedUserId { get; set; }
        public ApplicationUser AssignedUser { get; set; } = null!;

        public int? Estimation { get; set; }
        public int? EstimationUnitId { get; set; }
        public EstimationUnit? EstimationUnit { get; set; }

        public DateTime? StartedAt { get; set; }

        // Tracking the last successful completion of this stage (reset if rejected)
        public int? CompletedByUserId { get; set; }
        public virtual ApplicationUser? CompletedByUser { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
