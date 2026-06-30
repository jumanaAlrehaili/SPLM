using Domain.Entities.Projects;
using Microsoft.AspNetCore.Identity;

namespace Domain.IdentityEntities;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public virtual ICollection<IdentityUserRole<int>> UserRoles { get; set; } = new List<IdentityUserRole<int>>();

    public ICollection<ProjectResource> ProjectMemberships { get; set; } = new List<ProjectResource>();
    public ICollection<ProjectJoinRequest> JoinRequests { get; set; } = new List<ProjectJoinRequest>();
}
