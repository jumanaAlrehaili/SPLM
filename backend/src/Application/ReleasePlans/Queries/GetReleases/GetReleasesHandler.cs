using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ReleasePlans.Queries.GetReleases;

public class GetReleasesHandler : IRequestHandler<GetReleasesQuery, IEnumerable<ReleaseOutput>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetReleasesHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<ReleaseOutput>> Handle(GetReleasesQuery request, CancellationToken ct)
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
            .Where(r => r.ReleasePlanId == request.PlanId)
            .Select(r => new ReleaseOutput
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description,
                Status = r.Status.StatusName,
                StartDate = r.ReleaseStages.Min(s => (DateTime?)s.StartDate),
                EndDate = r.ReleaseStages.Max(s => (DateTime?)s.EndDate),
                StageCount = r.ReleaseStages.Count()
            })
            .ToListAsync(ct);
    }
}