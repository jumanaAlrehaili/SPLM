using Application.Shared.DTOs.Project;
using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetPendingRequests;

public class GetPendingRequestsHandler : IRequestHandler<GetPendingRequestsQuery, IEnumerable<JoinRequestOutput>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetPendingRequestsHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<JoinRequestOutput>> Handle(GetPendingRequestsQuery request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        return await _db.ProjectJoinRequests
            .AsNoTracking()
            .Where(r => r.Project.ProjectResources.Any(res => res.UserId == currentUserId && res.RoleId == RoleLookup.ProjectManager.Id)
                     && r.StatusId == RequestStatusesLookup.Pending.Id)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new JoinRequestOutput
            {
                Id          = r.Id,
                ProjectName = r.Project.Name,
                UserName    = r.User.FullName,
                JoinReason  = r.JoinReason,
                RequestedAt = r.CreatedAt,
                RoleName    = r.Role.Name
            })
            .ToListAsync(ct);
    }
}
