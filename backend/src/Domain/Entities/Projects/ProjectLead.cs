using Domain.Common;
using Domain.IdentityEntities;

namespace Domain.Entities.Projects
{
    public class ProjectLead : AuditedEntity
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = null!;

        public int LeadRoleId { get; set; }
        public LeadRole LeadRole { get; set; } = null!;

        public int UserId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}
