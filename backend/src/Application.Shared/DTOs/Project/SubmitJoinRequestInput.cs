using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.DTOs.Project
{
    public class SubmitJoinRequestInput
    {
        public int ProjectId { get; set; }
        public string JoinReason { get; set; } = string.Empty;
    }
}
