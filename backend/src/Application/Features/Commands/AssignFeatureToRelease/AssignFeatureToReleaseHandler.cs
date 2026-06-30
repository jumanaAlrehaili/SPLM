using Application.Features.Queries.GetFeatureById;
using Application.Shared.DTOs.Feature;
using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commands.AssignFeatureToRelease;

public class AssignFeatureToReleaseHandler : IRequestHandler<AssignFeatureToReleaseCommand, FeatureDetailOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public AssignFeatureToReleaseHandler(IAppDbContext db, ICurrentUserService currentUser, IMediator mediator)
    {
        _db = db;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<FeatureDetailOutput> Handle(AssignFeatureToReleaseCommand request, CancellationToken ct)
    {
        var isPM = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isPM)
            throw new UnauthorizedAccessException("Only a Project Manager of this project can perform this action.");

        var feature = await _db.Features
            .FirstOrDefaultAsync(f => f.Id == request.FeatureId && f.ProjectId == request.ProjectId, ct)
            ?? throw new KeyNotFoundException("Feature not found.");

        if (request.ReleaseId.HasValue)
        {
            var releaseBelongs = await _db.Releases
                .AnyAsync(r => r.Id == request.ReleaseId && r.ReleasePlan.ProjectId == request.ProjectId, ct);

            if (!releaseBelongs)
                throw new InvalidOperationException("The specified release does not belong to this project.");

            var releaseStageIds = await _db.ReleaseStages
                .Where(rs => rs.ReleaseId == request.ReleaseId)
                .Select(rs => rs.StageId)
                .ToListAsync(ct);

            var assignmentsToRemove = await _db.FeatureAssignments
                .Where(fa => fa.FeatureId == request.FeatureId && !releaseStageIds.Contains(fa.StageId))
                .ToListAsync(ct);

            if (assignmentsToRemove.Any())
                _db.FeatureAssignments.RemoveRange(assignmentsToRemove);
        }

        feature.ReleaseId       = request.ReleaseId;
        feature.UpdatedAt       = DateTime.UtcNow;
        feature.UpdatedByUserId = _currentUser.UserId;

        await _db.SaveChangesAsync(ct);

        return await _mediator.Send(new GetFeatureByIdQuery(request.ProjectId, feature.Id), ct)
            ?? throw new Exception("Failed to retrieve updated feature.");
    }
}
