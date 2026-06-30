using Application.Shared.Interfaces;
using Domain.Entities.Features;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commands.AssignUserToStage;

public class AssignUserToStageHandler : IRequestHandler<AssignUserToStageCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public AssignUserToStageHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(AssignUserToStageCommand request, CancellationToken ct)
    {
        var leadRoleConfig = await _db.LeadRoles
            .AsNoTracking()
            .Where(lr => lr.StageId == request.StageId)
            .Select(lr => new
            {
                lr.Id,
                lr.Name,
                lr.AssigneeRoleId
            })
            .FirstOrDefaultAsync(ct)
            ?? throw new KeyNotFoundException("No configuration found for this stage.");

        var isPM = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isPM)
        {
            var isLead = await _db.ProjectLeads
                .AnyAsync(pl => pl.ProjectId == request.ProjectId
                             && pl.UserId == _currentUser.UserId
                             && pl.LeadRoleId == leadRoleConfig.Id, ct);

            if (!isLead)
                throw new UnauthorizedAccessException($"Only the '{leadRoleConfig.Name}' can manage assignments for this stage.");
        }

        var feature = await _db.Features
            .FirstOrDefaultAsync(f => f.Id == request.FeatureId && f.ProjectId == request.ProjectId, ct)
            ?? throw new KeyNotFoundException("Feature not found in this project.");

        if (feature.ReleaseId.HasValue)
        {
            var stageExistsInRelease = await _db.ReleaseStages
                .AnyAsync(rs => rs.ReleaseId == feature.ReleaseId && rs.StageId == request.StageId, ct);

            if (!stageExistsInRelease)
                throw new InvalidOperationException("This stage is not included in the feature's release.");
        }

        var isValidResource = await _db.ProjectResources
            .AnyAsync(pr => pr.ProjectId == request.ProjectId
                         && pr.UserId == request.AssignedUserId
                         && pr.RoleId == leadRoleConfig.AssigneeRoleId, ct);

        if (!isValidResource)
            throw new InvalidOperationException("This user is not registered in the project or doesn't have the required role.");

        var assignment = await _db.FeatureAssignments
            .FirstOrDefaultAsync(fa => fa.FeatureId == request.FeatureId && fa.StageId == request.StageId, ct);

        if (assignment != null && assignment.CompletedAt.HasValue)
        {
            throw new InvalidOperationException("Cannot change the assigned user for an already completed stage.");
        }

        if (assignment == null)
        {
            _db.FeatureAssignments.Add(new FeatureAssignment
            {
                FeatureId       = request.FeatureId,
                StageId         = request.StageId,
                AssignedUserId  = request.AssignedUserId,
                CreatedAt       = DateTime.UtcNow,
                CreatedByUserId = _currentUser.UserId
            });
        }
        else
        {
            assignment.AssignedUserId  = request.AssignedUserId;
            assignment.UpdatedAt       = DateTime.UtcNow;
            assignment.UpdatedByUserId = _currentUser.UserId;
        }

        await _db.SaveChangesAsync(ct);
    }
}
