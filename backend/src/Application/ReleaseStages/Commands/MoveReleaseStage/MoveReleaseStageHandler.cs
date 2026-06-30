using Application.Common;
using Application.Shared.DTOs.Notification;
using Application.Shared.Interfaces;
using Domain.Entities.Notifications;
using Domain.Entities.Releases;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ReleaseStages.Commands.MoveReleaseStage;

public class MoveReleaseStageHandler : IRequestHandler<MoveReleaseStageCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IRealtimeNotificationService _realtime;

    public MoveReleaseStageHandler(IAppDbContext db, ICurrentUserService currentUser, IRealtimeNotificationService realtime)
    {
        _db = db;
        _currentUser = currentUser;
        _realtime = realtime;
    }

    public async Task Handle(MoveReleaseStageCommand request, CancellationToken ct)
    {
        var isMember = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId && r.UserId == _currentUser.UserId, ct);
        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this project.");

        var planBelongs = await _db.ReleasePlans
            .AnyAsync(p => p.Id == request.PlanId && p.ProjectId == request.ProjectId, ct);
        if (!planBelongs)
            throw new KeyNotFoundException("Release plan not found in this project.");

        var releaseBelongs = await _db.Releases
            .AnyAsync(r => r.Id == request.ReleaseId && r.ReleasePlanId == request.PlanId, ct);
        if (!releaseBelongs)
            throw new KeyNotFoundException("Release not found in this plan.");

        var stage = await _db.ReleaseStages
            .FirstOrDefaultAsync(s => s.Id == request.StageId && s.ReleaseId == request.ReleaseId, ct)
            ?? throw new KeyNotFoundException("Release stage not found.");

        // 2-tier permission check (PM or Stage Lead)
        var isPM = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isPM)
        {
            var isLead = await _db.ProjectLeads
                .AnyAsync(pl => pl.ProjectId == request.ProjectId
                             && pl.UserId == _currentUser.UserId
                             && pl.LeadRole.StageId == stage.StageId, ct);

            if (!isLead)
                throw new UnauthorizedAccessException("Only the Project Manager or the designated Stage Lead can perform this action.");
        }

        // Ensure all previous stages in sequence are completed
        var previousIncomplete = await _db.ReleaseStages
            .AnyAsync(s => s.ReleaseId == request.ReleaseId
                        && s.Sequence < stage.Sequence
                        && s.StatusId != ReleaseStageStatusLookup.Completed.Id, ct);

        if (previousIncomplete)
            throw new InvalidOperationException("Cannot advance this stage. Previous stages must be completed first.");

        // Status transition logic
        int oldStatusId = stage.StatusId;
        int nextStatusId;

        if (oldStatusId == ReleaseStageStatusLookup.NotStarted.Id)
            nextStatusId = ReleaseStageStatusLookup.InProgress.Id;
        else if (oldStatusId == ReleaseStageStatusLookup.InProgress.Id)
            nextStatusId = ReleaseStageStatusLookup.Completed.Id;
        else if (oldStatusId == ReleaseStageStatusLookup.Completed.Id)
            throw new InvalidOperationException("This stage is already Completed.");
        else
            throw new InvalidOperationException("Unknown stage status.");

        stage.StatusId = nextStatusId;
        stage.UpdatedAt = DateTime.UtcNow;
        stage.UpdatedByUserId = _currentUser.UserId;

             var oldStartDate = stage.StartDate;
        var oldEndDate = stage.EndDate;

        if (nextStatusId == ReleaseStageStatusLookup.InProgress.Id)
        {
            var start = DateTime.UtcNow.Date;
            var startOnly = DateOnly.FromDateTime(start);

            var holidayRanges = await _db.Holidays
                .AsNoTracking()
                .Where(h => h.EndDate >= startOnly)
                .Select(h => new { h.StartDate, h.EndDate })
                .ToListAsync(ct);

            stage.StartDate = start;
            stage.EndDate = WorkingDaysCalculator.Calculate(
                start,
                stage.WorkingDays,
                holidayRanges.Select(h => (h.StartDate, h.EndDate)));
        }

        // If all stages are now completed, auto-complete the release
        if (nextStatusId == ReleaseStageStatusLookup.Completed.Id)
        {
            var anyOtherUncompleted = await _db.ReleaseStages
                .AnyAsync(s => s.ReleaseId == request.ReleaseId
                            && s.Id != request.StageId
                            && s.StatusId != ReleaseStageStatusLookup.Completed.Id, ct);

            if (!anyOtherUncompleted)
            {
                var release = await _db.Releases.FirstOrDefaultAsync(r => r.Id == request.ReleaseId, ct);
                if (release != null)
                {
                    release.StatusId = FeatureStatusLookup.Completed.Id;
                    release.UpdatedAt = DateTime.UtcNow;
                    release.UpdatedByUserId = _currentUser.UserId;
                }
            }
        }

        _db.ReleaseStageHistories.Add(new ReleaseStageHistory
        {
            ReleaseStageId = stage.Id,
            OldStatusId = oldStatusId,
            NewStatusId = nextStatusId,
            OldStartDate = oldStartDate,
            NewStartDate = stage.StartDate,
            OldEndDate = oldEndDate,
            NewEndDate = stage.EndDate,
            ChangeType = ReleaseStageChangeTypeLookup.Updated.Id,
            Notes = request.Notes ?? "Stage advanced to next status.",
            ChangedAt = DateTime.UtcNow,
            ChangedByUserId = _currentUser.UserId
        });

        // Build notifications for next stage leads when current stage is completed
        var notificationsToSend = new List<(int UserId, Notification Entity)>();

        if (nextStatusId == ReleaseStageStatusLookup.Completed.Id)
        {
            var nextStage = await _db.ReleaseStages
                .Where(s => s.ReleaseId == request.ReleaseId && s.Sequence == stage.Sequence + 1)
                .Select(s => new { s.Id, s.StageId })
                .FirstOrDefaultAsync(ct);

            if (nextStage != null)
            {
                var leadsToNotify = await _db.ProjectLeads
                    .Where(pl => pl.ProjectId == request.ProjectId
                              && pl.LeadRole.StageId == nextStage.StageId)
                    .Select(pl => pl.UserId)
                    .Distinct()
                    .ToListAsync(ct);

                var stageName = await _db.Stages
                    .Where(s => s.Id == nextStage.StageId)
                    .Select(s => s.StageName)
                    .FirstOrDefaultAsync(ct) ?? "next stage";

                foreach (var userId in leadsToNotify)
                {
                    var entity = new Notification
                    {
                        UserId    = userId,
                        FeatureId = null,
                        Message   = $"Release stage '{stageName}' is now ready for your action.",
                        IsRead    = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    _db.Notifications.Add(entity);
                    notificationsToSend.Add((userId, entity));
                }
            }
        }

        // Save everything in one transaction
        await _db.SaveChangesAsync(ct);

        // Push real-time notifications after DB save (best-effort — offline users get it on next login)
        foreach (var (userId, entity) in notificationsToSend)
        {
            await _realtime.SendNotificationAsync(userId, new NotificationOutput
            {
                Id        = entity.Id,
                Message   = entity.Message,
                IsRead    = false,
                CreatedAt = entity.CreatedAt
            });
        }
    }
}
