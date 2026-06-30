using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Shared.DTOs.Project
{
    public record ProjectOutput
    {
        public int Id { get; init; }
        public string? Name { get; init; }
        public string? Description { get; init; }
        public decimal? Budget { get; init; }
        public int? Duration { get; init; }
        public string? DurationUnit { get; init; }
        public DateTime CreatedAt { get; init; }
        public string? CreatorName { get; init; }
        public int ResourcesCount { get; init; }

        public int MembershipStatusId { get; init; } //user current relation (None, Pending, Member, Owner).
        public List<ProjectResourceDto> Resources { get; set; } = new List<ProjectResourceDto>();
        public List<ProjectTeamsOutput> GroupedResources { get; set; }
    }

    public class ProjectResourceDto
    {
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int RoleId { get; set; }
        public string? RoleName { get; set; }
        public bool IsLeader { get; set; }
    }
}
