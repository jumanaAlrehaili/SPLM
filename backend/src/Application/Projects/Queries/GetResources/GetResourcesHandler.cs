using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Project;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetResources;

public class GetResourcesHandler : IRequestHandler<GetResourcesQuery, PagedResult<ResourceOutput>>
{
    private readonly IAppDbContext _db;

    public GetResourcesHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<ResourceOutput>> Handle(GetResourcesQuery request, CancellationToken ct)
    {
        var query = _db.ProjectResources.AsNoTracking();

        if (request.ProjectId.HasValue)
            query = query.Where(r => r.ProjectId == request.ProjectId.Value);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(r => r.Project.Name)
            .ThenBy(r => r.User.FullName)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new ResourceOutput
            {
                UserId      = r.UserId,
                FullName    = r.User.FullName,
                Email       = r.User.Email,
                RoleId      = r.RoleId,
                RoleName    = r.Role.Name,
                ProjectId   = r.ProjectId,
                ProjectName = r.Project.Name
            })
            .ToListAsync(ct);

        return new PagedResult<ResourceOutput>
        {
            Items      = items,
            TotalCount = totalCount,
            Page       = request.Page,
            PageSize   = request.PageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize)
        };
    }
}
