using Application.Common;
using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using Domain.Entities.Releases;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.ReleaseStages.Commands.UpdateReleaseStage;

public class UpdateReleaseStageHandler : IRequestHandler<UpdateReleaseStageCommand, ReleaseStageOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateReleaseStageHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ReleaseStageOutput> Handle(UpdateReleaseStageCommand request, CancellationToken ct)
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

        var isPM = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.ProjectManager.Id, ct);

        // 2-tier permission check (PM or Stage Lead)
        if (!isPM)
        {
            var isLead = await _db.ProjectLeads
                .AnyAsync(pl => pl.ProjectId == request.ProjectId
                             && pl.UserId == _currentUser.UserId
                             && pl.LeadRole.StageId == stage.StageId, ct);

            if (!isLead)
                throw new UnauthorizedAccessException("Only the Project Manager or the designated Stage Lead can perform this action.");
        }

        // LogStageChange
        var oldStartDate = stage.StartDate;
        var oldEndDate = stage.EndDate;

        stage.StageName = request.Input.StageName;
        stage.WorkingDays = request.Input.WorkingDays;

        // If the stage has already started, recompute its EndDate from the new working-days estimate.
        if (stage.StatusId == ReleaseStageStatusLookup.InProgress.Id && stage.StartDate.HasValue)
        {
            var startOnly = DateOnly.FromDateTime(stage.StartDate.Value);
            var holidayRanges = await _db.Holidays
                .AsNoTracking()
                .Where(h => h.EndDate >= startOnly)
                .Select(h => new { h.StartDate, h.EndDate })
                .ToListAsync(ct);

            stage.EndDate = WorkingDaysCalculator.Calculate(
                stage.StartDate.Value,
                stage.WorkingDays,
                holidayRanges.Select(h => (h.StartDate, h.EndDate)));
        }

        stage.UpdatedAt = DateTime.UtcNow;
        stage.UpdatedByUserId = _currentUser.UserId;

        _db.ReleaseStageHistories.Add(new ReleaseStageHistory
        {
            ReleaseStageId = stage.Id,
            OldStartDate = oldStartDate,
            NewStartDate = stage.StartDate,
            OldEndDate = oldEndDate,
            NewEndDate = stage.EndDate,
            ChangeType = ReleaseStageChangeTypeLookup.Updated.Id,
            Notes = "Stage details updated.",
            ChangedAt = DateTime.UtcNow,
            ChangedByUserId = _currentUser.UserId
        });

        await _db.SaveChangesAsync(ct);

        return await _db.ReleaseStages
            .AsNoTracking()
            .Where(s => s.Id == stage.Id)
            .Select(s => new ReleaseStageOutput
            {
                Id = s.Id,
                ReleaseId = s.ReleaseId,
                StageId = s.StageId,
                StageName = s.StageName,
                Sequence = s.Sequence,
                WorkingDays = s.WorkingDays,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                StatusId = s.StatusId
            })
            .FirstAsync(ct);
    }
}