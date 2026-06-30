using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Project;
using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetAllProjects;

public class GetAllProjectsHandler : IRequestHandler<GetAllProjectsQuery, PagedResult<ProjectOutput>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAllProjectsHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<ProjectOutput>> Handle(GetAllProjectsQuery request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var query = _db.Projects.AsNoTracking();

        var totalCount = await query.CountAsync(ct);

        var projects = await query
            .OrderByDescending(p => p.CreatedAt)
            .ThenByDescending(p => p.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
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
                       ResourcesCount = p.ProjectResources.Count(),

                       UserRoleId = p.ProjectResources
                           .Where(r => r.UserId == currentUserId)
                           .Select(r => (int?)r.RoleId)
                           .FirstOrDefault(),

                       HasPendingJoinRequest = p.ProjectJoinRequests.Any(j =>
                           j.UserId == currentUserId &&
                           j.StatusId == RequestStatusesLookup.Pending.Id)
                   })
                   .ToListAsync(ct);

        var items = projects.Select(p => new ProjectOutput
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Budget = p.Budget,
            Duration = p.Duration,
            DurationUnit = p.DurationUnit,
            CreatedAt = p.CreatedAt,
            CreatorName = p.CreatorName,
            ResourcesCount = p.ResourcesCount,
            MembershipStatusId = GetMembershipStatus(p.UserRoleId, p.HasPendingJoinRequest)
        }).ToList();


        return new PagedResult<ProjectOutput>
        {
            Items      = items,
            TotalCount = totalCount,
            Page       = request.Page,
            PageSize   = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }

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
