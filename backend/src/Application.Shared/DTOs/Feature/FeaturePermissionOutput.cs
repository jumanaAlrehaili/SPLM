using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.DTOs.Feature
{
    public class FeaturePermissionOutput
    {
        public List<int> LeadStageIds { get; set; } = new List<int>();
        public bool IsPM { get; set; }
    }
}
