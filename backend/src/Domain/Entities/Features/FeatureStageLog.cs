using Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Entities.Features
{
    public class FeatureStageLog
    {
        public int Id { get; set; }
        public int FeatureId { get; set; }
        public virtual Feature Feature { get; set; } = null!;

        public int StageId { get; set; }
        public virtual Stage Stage { get; set; } = null!;

        public int UserId { get; set; }
        public virtual ApplicationUser User { get; set; } = null!;

        public string Action { get; set; } = null!; // "COMPLETED" or "REJECTED"
        public string? Comment { get; set; }

        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public static class StageActions
    {
        public const string Started = "STARTED";
        public const string Completed = "COMPLETED";
        public const string Rejected = "REJECTED";
    }
}
