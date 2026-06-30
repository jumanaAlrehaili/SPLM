using Domain.Common;
using Domain.Entities.Features;

namespace Domain.Entities.Releases
{
    public class Release : AuditedEntity
    {
        public int Id { get; set; }

        public int ReleasePlanId { get; set; }
        public ReleasePlan ReleasePlan { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public int StatusId { get; set; }
        public Status Status { get; set; } = null!;

        public ICollection<ReleaseStage> ReleaseStages { get; set; } = new HashSet<ReleaseStage>();
        public ICollection<Feature> Features { get; set; } = new HashSet<Feature>();
    }
}
