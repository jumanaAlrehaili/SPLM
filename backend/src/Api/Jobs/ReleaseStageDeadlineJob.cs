using Application.Shared.DTOs.Notification;
using Application.Shared.Interfaces;
using Domain.Entities.Notifications;
using Domain.Lookups;
using Hangfire;
using Microsoft.EntityFrameworkCore;

namespace Api.Jobs;

/// <summary>
/// Hangfire recurring job: scans for release stages that are In Progress and whose
/// EndDate is within the next day, then warns the stage leads. Each stage is warned only
/// once (tracked via ReleaseStage.DueSoonNotifiedAt).
/// </summary>
public class ReleaseStageDeadlineJob
{
    public const string RecurringJobId = "release-stage-deadline-monitor";

    private readonly IAppDbContext _db;
    private readonly IRealtimeNotificationService _realtime;
    private readonly ILogger<ReleaseStageDeadlineJob> _logger;

    public ReleaseStageDeadlineJob(
        IAppDbContext db,
        IRealtimeNotificationService realtime,
        ILogger<ReleaseStageDeadlineJob> logger)
    {
        _db = db;
        _realtime = realtime;
        _logger = logger;
    }

    [DisableConcurrentExecution(timeoutInSeconds: 5 * 60)]
    public async Task RunAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var warningThreshold = now.AddDays(1);

        var dueSoonStages = await _db.ReleaseStages
            .Include(s => s.Stage)
            .Include(s => s.Release).ThenInclude(r => r.ReleasePlan)
            .Where(s => s.StatusId == ReleaseStageStatusLookup.InProgress.Id
                     && s.EndDate != null
                     && s.EndDate <= warningThreshold
                     && s.DueSoonNotifiedAt == null)
            .ToListAsync(ct);

        if (dueSoonStages.Count == 0)
            return;

        _logger.LogInformation("Found {Count} release stage(s) due soon to notify.", dueSoonStages.Count);

        var notificationsToSend = new List<(int UserId, Notification Entity)>();

        foreach (var stage in dueSoonStages)
        {
            var projectId = stage.Release.ReleasePlan.ProjectId;

            var leadsToNotify = await _db.ProjectLeads
                .Where(pl => pl.ProjectId == projectId && pl.LeadRole.StageId == stage.StageId)
                .Select(pl => pl.UserId)
                .Distinct()
                .ToListAsync(ct);

            var stageName = stage.StageName ?? stage.Stage.StageName ?? "stage";
            var dueDate = stage.EndDate!.Value.ToString("yyyy-MM-dd");

            foreach (var userId in leadsToNotify)
            {
                var entity = new Notification
                {
                    UserId    = userId,
                    FeatureId = null,
                    Message   = $"Release stage '{stageName}' is due soon (due {dueDate}).",
                    IsRead    = false,
                    CreatedAt = now
                };

                _db.Notifications.Add(entity);
                notificationsToSend.Add((userId, entity));
            }

            stage.DueSoonNotifiedAt = now;
        }

        await _db.SaveChangesAsync(ct);

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
