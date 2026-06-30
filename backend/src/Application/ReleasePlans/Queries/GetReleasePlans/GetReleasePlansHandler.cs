using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ReleasePlans.Queries.GetReleasePlans;

public class GetReleasePlansHandler : IRequestHandler<GetReleasePlansQuery, IEnumerable<ReleasePlanOutput>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetReleasePlansHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<ReleasePlanOutput>> Handle(GetReleasePlansQuery request, CancellationToken ct)
    {
        var isMember = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId && r.UserId == _currentUser.UserId, ct);

        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this project.");

        return await _db.ReleasePlans
            .AsNoTracking()
            .Where(p => p.ProjectId == request.ProjectId)
            .Select(p => new ReleasePlanOutput
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ReleaseCount = p.Releases.Count(),
                CreatedBy = p.CreatedByUser.FullName,
                CreatedAt = p.CreatedAt
            })
            .ToListAsync(ct);
    }
}