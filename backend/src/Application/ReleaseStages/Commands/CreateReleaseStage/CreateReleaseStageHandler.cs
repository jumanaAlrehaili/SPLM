using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using Domain.Entities.Releases;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ReleaseStages.Commands.CreateReleaseStage;

public class CreateReleaseStageHandler : IRequestHandler<CreateReleaseStageCommand, ReleaseStageOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateReleaseStageHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ReleaseStageOutput> Handle(CreateReleaseStageCommand request, CancellationToken ct)
    {
        var planBelongs = await _db.ReleasePlans
            .AnyAsync(p => p.Id == request.PlanId && p.ProjectId == request.ProjectId, ct);
        if (!planBelongs)
            throw new KeyNotFoundException("Release plan not found in this project.");

        var releaseBelongs = await _db.Releases
            .AnyAsync(r => r.Id == request.ReleaseId && r.ReleasePlanId == request.PlanId, ct);
        if (!releaseBelongs)
            throw new KeyNotFoundException("Release not found in this plan.");

        // Only the Project Manager can create stages.
        var isPM = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.ProjectManager.Id, ct);
        if (!isPM)
            throw new UnauthorizedAccessException("Only the Project Manager can create release stages.");

        // Validate the chosen stage exists in the catalog.
        var stage = await _db.Stages
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Input.StageId, ct)
            ?? throw new InvalidOperationException("Invalid stage selected.");

        // Prevent the same catalog stage being added twice to the same release.
        var alreadyExists = await _db.ReleaseStages
            .AnyAsync(rs => rs.ReleaseId == request.ReleaseId && rs.StageId == request.Input.StageId, ct);
        if (alreadyExists)
            throw new InvalidOperationException("This stage already exists in the release.");

        // Auto-assign the next sequence number (creation order).
        var maxSequence = await _db.ReleaseStages
            .Where(rs => rs.ReleaseId == request.ReleaseId)
            .Select(rs => (int?)rs.Sequence)
            .MaxAsync(ct) ?? 0;

        var releaseStage = new ReleaseStage
        {
            ReleaseId       = request.ReleaseId,
            StageId         = request.Input.StageId,
            StageName       = string.IsNullOrWhiteSpace(request.Input.StageName) ? stage.StageName : request.Input.StageName,
            Sequence        = maxSequence + 1,
            WorkingDays     = request.Input.WorkingDays,
            StartDate       = null,
            EndDate         = null,
            StatusId        = ReleaseStageStatusLookup.NotStarted.Id,
            CreatedAt       = DateTime.UtcNow,
            CreatedByUserId = _currentUser.UserId
        };

        _db.ReleaseStages.Add(releaseStage);
        await _db.SaveChangesAsync(ct);

        return new ReleaseStageOutput
        {
            Id          = releaseStage.Id,
            ReleaseId   = releaseStage.ReleaseId,
            StageId     = releaseStage.StageId,
            StageName   = releaseStage.StageName,
            Sequence    = releaseStage.Sequence,
            WorkingDays = releaseStage.WorkingDays,
            StartDate   = releaseStage.StartDate,
            EndDate     = releaseStage.EndDate,
            StatusId    = releaseStage.StatusId
        };
    }
}
