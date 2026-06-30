using Domain.Entities.Features;
using Domain.IdentityEntities;

namespace Domain.Entities.Projects
{
    public class LeadRole
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public int StageId { get; set; }
        public Stage Stage { get; set; } = null!;
        public int AssigneeRoleId { get; set; }
        public virtual ApplicationRole AssigneeRole { get; set; } = null!;
        public virtual ICollection<ProjectLead> ProjectLeads { get; set; } = new List<ProjectLead>();
    }
}
