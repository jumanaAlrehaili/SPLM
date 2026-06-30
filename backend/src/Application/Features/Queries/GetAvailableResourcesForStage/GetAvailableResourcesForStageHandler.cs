using Application.Shared.DTOs.Feature;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Queries.GetAvailableResourcesForStage;

public class GetAvailableResourcesForStageHandler
    : IRequestHandler<GetAvailableResourcesForStageQuery, IEnumerable<AvailableStageResourceDto>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetAvailableResourcesForStageHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<AvailableStageResourceDto>> Handle(
        GetAvailableResourcesForStageQuery request, CancellationToken ct)
    {
        var isMember = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId && r.UserId == _currentUser.UserId, ct);

        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this project.");

        var assigneeRoleId = await _db.LeadRoles
            .AsNoTracking()
            .Where(lr => lr.StageId == request.StageId)
            .Select(lr => (int?)lr.AssigneeRoleId)
            .FirstOrDefaultAsync(ct);

        if (assigneeRoleId == null)
            throw new KeyNotFoundException("No configuration found for this stage.");

        return await _db.ProjectResources
            .AsNoTracking()
            .Where(pr => pr.ProjectId == request.ProjectId && pr.RoleId == assigneeRoleId.Value)
            .Select(pr => new AvailableStageResourceDto(pr.UserId, pr.User.FullName))
            .ToListAsync(ct);
    }
}
