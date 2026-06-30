using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ReleasePlans.Queries.GetReleasePlanById;

public class GetReleasePlanByIdHandler : IRequestHandler<GetReleasePlanByIdQuery, ReleasePlanDetailOutput?>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetReleasePlanByIdHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }


    public async Task<ReleasePlanDetailOutput?> Handle(GetReleasePlanByIdQuery request, CancellationToken ct)
    {
        var isMember = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId && r.UserId == _currentUser.UserId, ct);

        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this project.");

        return await _db.ReleasePlans
            .AsNoTracking()
            .Where(p => p.Id == request.PlanId && p.ProjectId == request.ProjectId)
            .Select(p => new ReleasePlanDetailOutput
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                CreatedBy = p.CreatedByUser.FullName,
                CreatedAt = p.CreatedAt,
                Releases = p.Releases.Select(r => new ReleaseOutput
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description,
                    Status = r.Status.StatusName,
                    StartDate = r.ReleaseStages.Min(s => (DateTime?)s.StartDate),
                    EndDate = r.ReleaseStages.Max(s => (DateTime?)s.EndDate),
                    StageCount = r.ReleaseStages.Count()
                }).ToList()
            })
            .FirstOrDefaultAsync(ct);
    }
}