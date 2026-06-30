using Application.ReleasePlans.Queries.GetReleaseById;
using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ReleasePlans.Commands.UpdateRelease;

public class UpdateReleaseHandler : IRequestHandler<UpdateReleaseCommand, ReleaseDetailOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateReleaseHandler(IAppDbContext db, ICurrentUserService currentUser, IMediator mediator)
    {
        _db = db;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<ReleaseDetailOutput> Handle(UpdateReleaseCommand request, CancellationToken ct)
    {
        var releaseName = request.Input.Name.Trim();

        if (string.IsNullOrWhiteSpace(releaseName))
            throw new InvalidOperationException("Release name is required.");

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

        var nameExists = await _db.Releases
            .AnyAsync(r =>
                r.ReleasePlanId == request.PlanId &&
                r.Id != request.ReleaseId &&
                r.Name == releaseName,
                ct);

        if (nameExists)
            throw new InvalidOperationException(
                $"A release with the name '{releaseName}' already exists in this plan.");

        release.Name = releaseName;
        release.Description = request.Input.Description;
        release.UpdatedAt = DateTime.UtcNow;
        release.UpdatedByUserId = _currentUser.UserId;

        await _db.SaveChangesAsync(ct);

        return await _mediator.Send(new GetReleaseByIdQuery(request.ProjectId, request.PlanId, release.Id), ct)
            ?? throw new Exception("Failed to retrieve updated release.");
    }
}