using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ReleasePlans.Commands.DeleteRelease;

public class DeleteReleaseHandler : IRequestHandler<DeleteReleaseCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteReleaseHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteReleaseCommand request, CancellationToken ct)
    {
        var isPM = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isPM)
            throw new UnauthorizedAccessException("Only the Project Manager can perform this action.");

        var belongs = await _db.ReleasePlans
            .AnyAsync(p => p.Id == request.PlanId && p.ProjectId == request.ProjectId, ct);

        if (!belongs)
            throw new KeyNotFoundException("Release plan not found in this project.");

        var release = await _db.Releases
            .FirstOrDefaultAsync(r => r.Id == request.ReleaseId && r.ReleasePlanId == request.PlanId, ct)
            ?? throw new KeyNotFoundException("Release not found.");

        _db.Releases.Remove(release);
        await _db.SaveChangesAsync(ct);
    }
}