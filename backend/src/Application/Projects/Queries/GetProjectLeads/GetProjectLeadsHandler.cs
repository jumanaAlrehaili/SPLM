using Application.Shared.DTOs.Project;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Queries.GetProjectLeads;

public class GetProjectLeadsHandler : IRequestHandler<GetProjectLeadsQuery, IEnumerable<ProjectLeadOutput>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetProjectLeadsHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<ProjectLeadOutput>> Handle(GetProjectLeadsQuery request, CancellationToken ct)
    {
        var isMember = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId && r.UserId == _currentUser.UserId, ct);

        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this project.");

        //return await _db.LeadRoles.AsNoTracking()
        //    .Select(lr => new ProjectLeadOutput
        //    {
        //        LeadRoleId   = lr.Id,
        //        LeadRoleName = lr.Name,
        //        StageId      = lr.StageId,
        //        UserId       = lr.ProjectLeads
        //                          .Where(pl => pl.ProjectId == request.ProjectId)
        //                          .Select(pl => (int?)pl.UserId)
        //                          .FirstOrDefault(),
        //        UserName     = lr.ProjectLeads
        //                          .Where(pl => pl.ProjectId == request.ProjectId)
        //                          .Select(pl => pl.User.FullName)
        //                          .FirstOrDefault()
        //    })
        //    .ToListAsync(ct);

        return await (
                from lr in _db.LeadRoles.AsNoTracking()
                join pl in _db.ProjectLeads
                        .AsNoTracking()
                        .Where(pl => pl.ProjectId == request.ProjectId)
                    on lr.Id equals pl.LeadRoleId into projectLeadGroup
                from pl in projectLeadGroup.DefaultIfEmpty() //sql = left join
                orderby lr.StageId, lr.Id
                select new ProjectLeadOutput
                {
                    LeadRoleId = lr.Id,
                    LeadRoleName = lr.Name,
                    StageId = lr.StageId,
                    UserId = pl == null ? null : pl.UserId,
                    UserName = pl == null ? null : pl.User.FullName
                }).ToListAsync(ct);
    }
}
