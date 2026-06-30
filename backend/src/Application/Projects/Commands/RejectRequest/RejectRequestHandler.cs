using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands.RejectRequest;

public class RejectRequestHandler : IRequestHandler<RejectRequestCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RejectRequestHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(RejectRequestCommand request, CancellationToken ct)
    {
        var joinRequest = await _db.ProjectJoinRequests
            .FirstOrDefaultAsync(r => r.Id == request.RequestId && r.StatusId == RequestStatusesLookup.Pending.Id, ct);

        if (joinRequest == null)
            throw new InvalidOperationException("Request not found or has already been processed.");

        var isManager = await _db.ProjectResources
            .AnyAsync(res => res.ProjectId == joinRequest.ProjectId
                          && res.UserId == _currentUser.UserId
                          && res.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isManager)
            throw new UnauthorizedAccessException("You do not have permission to reject requests for this project.");

        joinRequest.StatusId = RequestStatusesLookup.Rejected.Id;
        await _db.SaveChangesAsync(ct);
    }
}
