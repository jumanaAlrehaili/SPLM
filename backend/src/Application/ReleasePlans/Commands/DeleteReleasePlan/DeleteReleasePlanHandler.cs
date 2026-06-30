using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ReleasePlans.Commands.DeleteReleasePlan;

public class DeleteReleasePlanHandler : IRequestHandler<DeleteReleasePlanCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteReleasePlanHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteReleasePlanCommand request, CancellationToken ct)
    {
        var isPM = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isPM)
            throw new UnauthorizedAccessException("Only the Project Manager can perform this action.");

        var plan = await _db.ReleasePlans
            .FirstOrDefaultAsync(p => p.Id == request.PlanId && p.ProjectId == request.ProjectId, ct)
            ?? throw new KeyNotFoundException("Release plan not found.");

        _db.ReleasePlans.Remove(plan);
        await _db.SaveChangesAsync(ct);
    }
}