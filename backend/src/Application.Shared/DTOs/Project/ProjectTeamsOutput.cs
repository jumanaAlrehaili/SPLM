using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.DTOs.Project
{
    public class ProjectTeamsOutput
    {
        public string RoleName { get; set; }
        public List<ProjectResourceDto> Members { get; set; }
    }
}
