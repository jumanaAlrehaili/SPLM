using Application.Shared.Interfaces;
using Domain.Entities.Projects;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands.SubmitJoinRequest;

public class SubmitJoinRequestHandler : IRequestHandler<SubmitJoinRequestCommand, int>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SubmitJoinRequestHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(SubmitJoinRequestCommand request, CancellationToken ct)
    {
        var currentUserId  = _currentUser.UserId;
        var roleIdFromToken = _currentUser.RoleId;

        var projectExists = await _db.Projects.AsNoTracking().AnyAsync(p => p.Id == request.Input.ProjectId, ct);

        if (!projectExists)
            throw new KeyNotFoundException("Project not found.");

        var roleExists = await _db.Roles.AsNoTracking().AnyAsync(r => r.Id == roleIdFromToken, ct);

        if (!roleExists)
            throw new KeyNotFoundException("Role not found.");

        var isAlreadyMember = await _db.ProjectResources
            .AsNoTracking()
            .AnyAsync(r => r.ProjectId == request.Input.ProjectId && r.UserId == currentUserId, ct);

        if (isAlreadyMember)
            throw new InvalidOperationException("You are already part of this project.");

        var existingRequest = await _db.ProjectJoinRequests
            .AsNoTracking()
            .AnyAsync(r => r.ProjectId == request.Input.ProjectId && r.UserId == currentUserId && r.StatusId == RequestStatusesLookup.Pending.Id, ct);

        if (existingRequest)
            throw new InvalidOperationException("You already have a pending request for this project.");

        var joinRequest = new ProjectJoinRequest
        {
            ProjectId  = request.Input.ProjectId,
            UserId     = currentUserId,
            RoleId     = roleIdFromToken,
            JoinReason = request.Input.JoinReason,
            StatusId   = RequestStatusesLookup.Pending.Id,
            CreatedAt  = DateTime.UtcNow
        };

        _db.ProjectJoinRequests.Add(joinRequest);
        await _db.SaveChangesAsync(ct);

        return joinRequest.Id;
    }
}
