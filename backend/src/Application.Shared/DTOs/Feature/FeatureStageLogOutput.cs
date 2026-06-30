using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.DTOs.Feature
{
    public class FeatureStageLogOutput
    {
        public int Id { get; set; }
        public string Action { get; set; } = null!;
        public string? Comment { get; set; }
        public DateTime Timestamp { get; set; }

        public int UserId { get; set; }
        public string UserName { get; set; } = null!;

        public string StageName { get; set; } = null!;
    }
}
