using Domain.Common;
using Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Projects
{
    public class ProjectResource : AuditedEntity
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int RoleId { get; set; }
        public ApplicationRole Role { get; set; }

    }
}
