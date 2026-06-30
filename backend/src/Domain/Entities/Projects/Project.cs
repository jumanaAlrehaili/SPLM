using Domain.Common;
using Domain.Entities.Features;
using Domain.Entities.Releases;
using Domain.Lookups;

namespace Domain.Entities.Projects
{
    public class Project : AuditedEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? Budget { get; set; }
        public int? Duration { get; set; }

        public int? DurationUnitId { get; set; }
        public DurationUnit? DurationUnit { get; set; }

        public ICollection<ProjectResource> ProjectResources { get; set; } = new HashSet<ProjectResource>();
        public ICollection<ProjectJoinRequest> ProjectJoinRequests { get; set; } = new HashSet<ProjectJoinRequest>();
        public ICollection<ProjectLead> ProjectLeads { get; set; } = new HashSet<ProjectLead>();
        public ICollection<Feature> Features { get; set; } = new HashSet<Feature>();
        public ICollection<ReleasePlan> ReleasePlans { get; set; } = new HashSet<ReleasePlan>();
    }
}
