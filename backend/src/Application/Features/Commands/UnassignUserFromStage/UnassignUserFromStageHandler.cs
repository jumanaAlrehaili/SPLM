using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commands.UnassignUserFromStage;

public class UnassignUserFromStageHandler : IRequestHandler<UnassignUserFromStageCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UnassignUserFromStageHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(UnassignUserFromStageCommand request, CancellationToken ct)
    {
        var isPM = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isPM)
        {
            var requiredLeadRole = await _db.LeadRoles
                .AsNoTracking()
                .Where(lr => lr.StageId == request.StageId)
                .Select(lr => new
                {
                    lr.Id,
                    lr.Name
                })
               .FirstOrDefaultAsync(ct)
               ?? throw new KeyNotFoundException("No leadership role configured for this stage.");

            var isLead = await _db.ProjectLeads
                .AnyAsync(pl => pl.ProjectId == request.ProjectId
                             && pl.UserId == _currentUser.UserId
                             && pl.LeadRoleId == requiredLeadRole.Id, ct);

            if (!isLead)
                throw new UnauthorizedAccessException($"Only the '{requiredLeadRole.Name}' can manage assignments for this stage.");
        }

        var assignment = await _db.FeatureAssignments
            .FirstOrDefaultAsync(fa => fa.FeatureId == request.FeatureId 
                                    && fa.StageId == request.StageId 
                                    && fa.Feature.ProjectId == request.ProjectId, ct)
            ?? throw new KeyNotFoundException("No assignment found for this feature in the specified stage.");

        if (assignment.CompletedAt.HasValue)
            throw new InvalidOperationException("Cannot unassign user from an already completed stage.");

        _db.FeatureAssignments.Remove(assignment);
        await _db.SaveChangesAsync(ct);
    }
}
