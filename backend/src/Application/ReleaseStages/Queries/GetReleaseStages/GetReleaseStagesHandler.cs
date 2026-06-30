using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ReleaseStages.Queries.GetReleaseStages;

public class GetReleaseStagesHandler : IRequestHandler<GetReleaseStagesQuery, IEnumerable<ReleaseStageOutput>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetReleaseStagesHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<ReleaseStageOutput>> Handle(GetReleaseStagesQuery request, CancellationToken ct)
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

        return await _db.ReleaseStages
            .AsNoTracking()
            .Where(s => s.ReleaseId == request.ReleaseId)
            .OrderBy(s => s.Sequence)
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
            .ToListAsync(ct);
    }
}