using Application.Shared.Interfaces;
using Domain.Entities.Projects;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands.ApproveRequest;

public class ApproveRequestHandler : IRequestHandler<ApproveRequestCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public ApproveRequestHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(ApproveRequestCommand request, CancellationToken ct)
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
            throw new UnauthorizedAccessException("You do not have permission to approve requests for this project.");


        var isAlreadyMember = await _db.ProjectResources
            .AsNoTracking()
            .AnyAsync(res =>
                res.ProjectId == joinRequest.ProjectId &&
                res.UserId == joinRequest.UserId,
                ct);

        if (isAlreadyMember)
            throw new InvalidOperationException("User is already a member of this project.");

        joinRequest.StatusId = RequestStatusesLookup.Approved.Id;

        _db.ProjectResources.Add(new ProjectResource
        {
            ProjectId       = joinRequest.ProjectId,
            UserId          = joinRequest.UserId,
            RoleId          = joinRequest.RoleId,
            CreatedAt       = DateTime.UtcNow,
            CreatedByUserId = _currentUser.UserId
        });

        await _db.SaveChangesAsync(ct);
    }
}
