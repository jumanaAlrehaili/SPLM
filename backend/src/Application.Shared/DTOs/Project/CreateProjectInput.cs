using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.DTOs.Project
{
    public record CreateProjectInput(string Name, string Description, decimal? Budget = null, int? Duration = null, int? DurationUnit = null);
}
