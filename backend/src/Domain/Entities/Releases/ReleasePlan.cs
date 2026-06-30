using Domain.Common;
using Domain.Entities.Projects;

namespace Domain.Entities.Releases
{
    public class ReleasePlan : AuditedEntity
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<Release> Releases { get; set; } = new HashSet<Release>();
    }
}
