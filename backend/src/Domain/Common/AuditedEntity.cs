using System;
using System.Collections.Generic;
using System.Text;
using Domain.IdentityEntities;

namespace Domain.Common
{
    public abstract class CreationEntity
    {
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

    public abstract class AuditedEntity : CreationEntity
    {
        public int CreatedByUserId { get; set; }  //Foreign Key (for DB). 
        public virtual ApplicationUser CreatedByUser { get; set; } = null!; //Navigation Property (for fetching User details like Name/Email).

        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedByUserId { get; set; } // int? because it's null until someone updates the feature
        public virtual ApplicationUser? UpdatedByUser { get; set; }
    }
}
