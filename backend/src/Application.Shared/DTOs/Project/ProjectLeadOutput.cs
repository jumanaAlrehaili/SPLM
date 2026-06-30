using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.DTOs.Project
{
    public class ProjectLeadOutput
    {
        public int LeadRoleId { get; set; }
        public string LeadRoleName { get; set; } = string.Empty;
        public int StageId { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
    }
}
