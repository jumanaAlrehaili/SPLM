using Application.Shared.DTOs.Project;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetMyJoinRequests;

public class GetMyJoinRequestsHandler : IRequestHandler<GetMyJoinRequestsQuery, IEnumerable<JoinRequestOutput>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyJoinRequestsHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<JoinRequestOutput>> Handle(GetMyJoinRequestsQuery request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        return await _db.ProjectJoinRequests
            .AsNoTracking()
            .Where(r => r.UserId == currentUserId)
            .OrderByDescending(r => r.CreatedAt)
            .Select(r => new JoinRequestOutput
            {
                Id          = r.Id,
                ProjectName = r.Project.Name,
                UserName    = r.User.FullName,
                RoleName    = r.Role.Name,
                JoinReason  = r.JoinReason ?? string.Empty,
                RequestedAt = r.CreatedAt,
                Status      = r.Status.Name
            })
            .ToListAsync(ct);
    }
}
