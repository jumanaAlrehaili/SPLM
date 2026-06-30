using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Feature;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Queries.GetAllFeatures;

public class GetAllFeaturesHandler : IRequestHandler<GetAllFeaturesQuery, PagedResult<FeatureOutput>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAllFeaturesHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<FeatureOutput>> Handle(GetAllFeaturesQuery request, CancellationToken ct)
    {
        var isMember = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId && r.UserId == _currentUser.UserId, ct);

        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this project.");

        var query = _db.Features
            .AsNoTracking()
            .Where(f => f.ProjectId == request.ProjectId);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(f => f.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(f => new FeatureOutput
            {
                Id          = f.Id,
                Title       = f.Title,
                Description = f.Description,
                EpicLink    = f.EpicLink,
                PriorityId  = f.PriorityId,
                CompletedAt = f.CompletedAt,
                Status      = f.CurrentStatus.StatusName,
                Release     = f.Release == null ? null : new FeatureReleaseDto
                {
                    Id   = f.Release.Id,
                    Name = f.Release.Name
                },
                CreatedBy = f.CreatedByUser.FullName,
                CreatedAt = f.CreatedAt
            })
            .ToListAsync(ct);

        return new PagedResult<FeatureOutput>
        {
            Items      = items,
            TotalCount = totalCount,
            Page       = request.Page,
            PageSize   = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}
