using Domain.Common;
using Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Projects
{
    public class ProjectJoinRequest : CreationEntity
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
        public int RoleId { get; set; }
        public ApplicationRole Role { get; set; }

        public string JoinReason { get; set; }
        public int StatusId { get; set; }
        public RequestStatus Status { get; set; }
    }

}
