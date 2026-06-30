using Application.Shared.DTOs.Project;
using Application.Shared.Interfaces;
using Domain.Entities.Projects;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetProjectById;

public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, ProjectOutput?>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetProjectByIdHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ProjectOutput?> Handle(GetProjectByIdQuery request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        //var projectLeadUserIds = await _db.ProjectLeads
        //    .AsNoTracking()
        //    .Where(pl => pl.ProjectId == request.ProjectId)
        //    .Select(pl => pl.UserId)
        //    .ToListAsync(ct);

        var project = await _db.Projects
            .AsNoTracking()
            .Where(p => p.Id == request.ProjectId)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.Description,
                p.Budget,
                p.Duration,
                DurationUnit = p.DurationUnit != null ? p.DurationUnit.Name : null,
                p.CreatedAt,
                CreatorName = p.CreatedByUser.FullName,
                UserRoleInProjectResource = p.ProjectResources
                .Where(pr => pr.UserId == currentUserId)
                .Select(pr => (int?)pr.RoleId ).FirstOrDefault(),

                HasPendingJoinRequest = p.ProjectJoinRequests.Any(j => j.UserId == currentUserId && j.StatusId == RequestStatusesLookup.Pending.Id),
                Resources = p.ProjectResources.Select(r => new ProjectResourceDto
                {
                    UserId = r.UserId,
                    UserName = r.User.FullName,
                    RoleId = r.RoleId,
                    RoleName = r.Role.Name,
                    //IsLeader = projectLeadUserIds.Contains(r.UserId)
                    IsLeader = _db.ProjectLeads.Any(pl =>
                            pl.ProjectId == p.Id &&
                            pl.UserId == r.UserId)

                }).ToList()
            })
            .FirstOrDefaultAsync(ct); 


        var mapProject = project == null ? null : new ProjectOutput
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            Budget = project.Budget,
            Duration = project.Duration,
            DurationUnit = project.DurationUnit,
            CreatedAt = project.CreatedAt,
            CreatorName = project.CreatorName,
            MembershipStatusId = GetMembershipStatus(project.UserRoleInProjectResource, project.HasPendingJoinRequest),
            Resources = project.Resources
        };

        if (mapProject != null)
            mapProject.GroupedResources = GroupResources(mapProject.Resources);

        return mapProject;
    }

    private static List<ProjectTeamsOutput> GroupResources(IEnumerable<ProjectResourceDto> resources) =>
        resources
            .GroupBy(r => r.RoleId)
            .OrderBy(g => g.Key)
            .Select(g => new ProjectTeamsOutput
            {
                RoleName = g.First().RoleName,
                Members  = g.OrderByDescending(m => m.IsLeader)
                            .Select(m => new ProjectResourceDto
                            {
                                UserId   = m.UserId,
                                UserName = m.UserName,
                                IsLeader = m.IsLeader
                            }).ToList()
            })
            .ToList();

    private int GetMembershipStatus(int? userRoleId, bool hasPendingJoinRequest)
    {
        if (userRoleId == null)
            return hasPendingJoinRequest
                ? ProjectMembershipStatusLookup.Pending.Id
                : ProjectMembershipStatusLookup.None.Id;

        return userRoleId == RoleLookup.ProjectManager.Id
            ? ProjectMembershipStatusLookup.Owner.Id
            : ProjectMembershipStatusLookup.Member.Id;
    }
}
