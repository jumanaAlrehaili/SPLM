using Application.Shared.Interfaces;
using Domain.Entities.Features;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commands.RejectAndMoveToPreviousStage;

public class RejectAndMoveToPreviousStageHandler : IRequestHandler<RejectAndMoveToPreviousStageCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public RejectAndMoveToPreviousStageHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(RejectAndMoveToPreviousStageCommand request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var feature = await _db.Features
            .FirstOrDefaultAsync(f => f.Id == request.FeatureId && f.ProjectId == request.ProjectId, ct)
            ?? throw new KeyNotFoundException("Feature not found in this project.");

        var currentAssignment = await _db.FeatureAssignments
            .FirstOrDefaultAsync(fa => fa.FeatureId == request.FeatureId && fa.StageId == request.CurrentStageId, ct);

        if (currentAssignment == null || currentAssignment.AssignedUserId != currentUserId)
            throw new UnauthorizedAccessException("You are not the assigned user responsible for rejecting this stage.");

        if (currentAssignment.CompletedAt.HasValue)
            throw new InvalidOperationException("Cannot reject an already completed stage.");

        var now = DateTime.UtcNow;

        _db.FeatureStageLogs.Add(new FeatureStageLog
        {
            FeatureId = request.FeatureId,
            StageId   = request.CurrentStageId,
            UserId    = currentUserId,
            Timestamp = now,
            Action    = StageActions.Rejected,
            Comment   = request.RejectionComment
        });

        // Retrieve sequence for both Dev stage and the current rejecting stage
        var workflowSequences = await _db.Stages
            .AsNoTracking()
            .Where(s => s.Id == request.CurrentStageId || s.Id == StageLookup.Dev.Id) //CurrentStageId = Rejected stage (UAT or QA).
            .Select(s => new { s.Id, s.Sequence })
            .ToListAsync(ct);

        var rejectionStageSequence = workflowSequences.FirstOrDefault(s => s.Id == request.CurrentStageId)?.Sequence
            ?? throw new KeyNotFoundException("Rejecting stage not found."); // = 5(QA) or 6(UAT).

        var developmentStageSequence = workflowSequences.FirstOrDefault(s => s.Id == StageLookup.Dev.Id)?.Sequence
            ?? throw new KeyNotFoundException("Development stage not found."); // = 4(Dev)

        // Filter assignments to get only the stages involved in the rejection process
        var affectedAssignments = await _db.FeatureAssignments
            .Where(fa => fa.FeatureId == request.FeatureId 
                                      && _db.Stages.Any(s => s.Id == fa.StageId
                                                     && s.Sequence >= developmentStageSequence 
                                                     && s.Sequence <= rejectionStageSequence))
            .ToListAsync(ct);

        foreach (var assignment in affectedAssignments)
        {
            assignment.CompletedAt = null;
            assignment.CompletedByUserId = null;
            assignment.StartedAt = null;
            assignment.UpdatedAt = now;
            assignment.UpdatedByUserId = currentUserId;
        }

        feature.CurrentStatusId = FeatureStatusLookup.InProgress.Id;

        feature.UpdatedAt       = now;
        feature.UpdatedByUserId = currentUserId;

        await _db.SaveChangesAsync(ct);
    }
}
