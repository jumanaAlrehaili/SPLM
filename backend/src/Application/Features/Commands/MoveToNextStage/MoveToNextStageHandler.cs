using Application.Shared.Interfaces;
using Domain.Entities.Features;
using Domain.Entities.Projects;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commands.MoveToNextStage;

public class MoveToNextStageHandler : IRequestHandler<MoveToNextStageCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public MoveToNextStageHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(MoveToNextStageCommand request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var feature = await _db.Features
            .FirstOrDefaultAsync(f => f.Id == request.FeatureId && f.ProjectId == request.ProjectId, ct)
            ?? throw new KeyNotFoundException("Feature not found in this project.");

        var currentAssignment = await _db.FeatureAssignments
            .FirstOrDefaultAsync(fa => fa.FeatureId == request.FeatureId
                                    && fa.StageId == request.CurrentStageId
                                    && fa.AssignedUserId == currentUserId, ct)
            ?? throw new UnauthorizedAccessException("You are not the assigned user responsible for completing this stage.");

        if (currentAssignment.CompletedAt.HasValue)
            throw new InvalidOperationException("This stage has already been completed.");

        if (!currentAssignment.StartedAt.HasValue)
            throw new InvalidOperationException("You must start the stage before completing it.");

        var now = DateTime.UtcNow;

        var currentStageSequence = await _db.Stages
            .Where(s => s.Id == request.CurrentStageId)
            .Select(s => (int?)s.Sequence)
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException("Stage not found.");

        if (feature.ReleaseId.HasValue)
        {

            var releaseId = feature.ReleaseId.Value;

            var currentStageInRelease = await _db.ReleaseStages
                .AnyAsync(rs =>
                    rs.ReleaseId == releaseId &&
                    rs.StageId == request.CurrentStageId,
                    ct);

            if (!currentStageInRelease)
                throw new InvalidOperationException("This stage is not included in the feature's release.");

            // Get all stages within this release that must precede the current stage
            // Example: If current stage is Dev (Sequence: 3), required stages will be BA (Sequence: 1) and SA (Sequence: 2).
            var requiredStageIds = await _db.ReleaseStages
                .Where(rs => rs.ReleaseId == releaseId && rs.Stage.Sequence < currentStageSequence)
                .Select(rs => rs.StageId)
                .ToListAsync(ct);

            if (requiredStageIds.Any())
            {
                // Count how many of these preceding stages have been fully completed for this feature.
                var completedStagesCount = await _db.FeatureAssignments
                    .CountAsync(fa => fa.FeatureId == request.FeatureId
                                   && requiredStageIds.Contains(fa.StageId) //ex: check only the stages 1,2 and ignore qa(4).
                                   && fa.CompletedAt != null, ct);

                if (completedStagesCount < requiredStageIds.Count)
                {
                    throw new InvalidOperationException("Cannot complete this stage. All previous stages in the release lifecycle must be completed first.");
                }
            }
        }

        currentAssignment.CompletedByUserId = currentUserId;
        currentAssignment.CompletedAt = now;

        _db.FeatureStageLogs.Add(new FeatureStageLog
        {
            FeatureId = request.FeatureId,
            StageId = request.CurrentStageId,
            UserId = currentUserId,
            Timestamp = now,
            Action = StageActions.Completed,
            Comment = "Stage completed successfully"
        });

        // Update the global feature status
        if (request.CurrentStageId == StageLookup.BA.Id || request.CurrentStageId == StageLookup.SA.Id || request.CurrentStageId == StageLookup.UIUX.Id)
        {
            feature.CurrentStatusId = FeatureStatusLookup.InProgress.Id;
        }
        else if (request.CurrentStageId == StageLookup.Dev.Id || request.CurrentStageId == StageLookup.QA.Id)
        {
            feature.CurrentStatusId = FeatureStatusLookup.PendingReview.Id;
        }
        else if (request.CurrentStageId == StageLookup.UAT.Id)
        {
            feature.CurrentStatusId = FeatureStatusLookup.Completed.Id;
            feature.CompletedAt = now;
        }
        else
        {
            throw new InvalidOperationException("Invalid workflow stage execution pattern.");
        }

        feature.UpdatedAt = now;
        feature.UpdatedByUserId = currentUserId;

        await _db.SaveChangesAsync(ct);
    }
}
