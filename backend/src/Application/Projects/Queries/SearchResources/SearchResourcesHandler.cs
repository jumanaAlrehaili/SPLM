using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Project;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.SearchResources;

public class SearchResourcesHandler : IRequestHandler<SearchResourcesQuery, PagedResult<ResourceOutput>>
{
    private readonly IAppDbContext _db;

    public SearchResourcesHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<ResourceOutput>> Handle(SearchResourcesQuery request, CancellationToken ct)
    {
        var query = _db.ProjectResources
            .AsNoTracking()
            .Where(r => string.IsNullOrEmpty(request.SearchTerm)
                     || r.User.FullName.Contains(request.SearchTerm)
                     || (r.User.Email != null && r.User.Email.Contains(request.SearchTerm)))
            .Where(r => request.ProjectId == null || request.ProjectId.Count == 0 || request.ProjectId.Contains(r.ProjectId))
            .Where(r => request.RoleId == null || request.RoleId.Count == 0 || request.RoleId.Contains(r.RoleId));

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
