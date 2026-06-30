using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ReleasePlans.Queries.GetReleaseById;

public class GetReleaseByIdHandler : IRequestHandler<GetReleaseByIdQuery, ReleaseDetailOutput?>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetReleaseByIdHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ReleaseDetailOutput?> Handle(GetReleaseByIdQuery request, CancellationToken ct)
    {
        var isMember = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId && r.UserId == _currentUser.UserId, ct);

        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this project.");

        var belongs = await _db.ReleasePlans
            .AnyAsync(p => p.Id == request.PlanId && p.ProjectId == request.ProjectId, ct);

        if (!belongs)
            throw new KeyNotFoundException("Release plan not found in this project.");

        return await _db.Releases
            .AsNoTracking()
            .Where(r => r.Id == request.ReleaseId && r.ReleasePlanId == request.PlanId)
            .Select(r => new ReleaseDetailOutput
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Status = r.Status.StatusName,
                StartDate = r.ReleaseStages.Min(s => (DateTime?)s.StartDate),
                EndDate = r.ReleaseStages.Max(s => (DateTime?)s.EndDate),
                StageCount = r.ReleaseStages.Count(),
                Stages = r.ReleaseStages
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
                    }).ToList()
            })
            .FirstOrDefaultAsync(ct);
    }
}