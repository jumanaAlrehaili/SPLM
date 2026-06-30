using Application.Shared.DTOs.Feature;
using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Queries.GetFeatureById;

public class GetFeatureByIdHandler : IRequestHandler<GetFeatureByIdQuery, FeatureDetailOutput?>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetFeatureByIdHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<FeatureDetailOutput?> Handle(GetFeatureByIdQuery request, CancellationToken ct)
    {
        var currentUserId = _currentUser.UserId;

        var userRoleId = await _db.ProjectResources
            .AsNoTracking()
            .Where(r => r.ProjectId == request.ProjectId && r.UserId == currentUserId)
            .Select(r => (int?)r.RoleId)
            .FirstOrDefaultAsync(ct);

        if (userRoleId == null)
            throw new UnauthorizedAccessException("You are not a member of this project.");

        var isPM = userRoleId == RoleLookup.ProjectManager.Id;

        var feature = await _db.Features
            .AsNoTracking()
            .Where(f => f.Id == request.FeatureId && f.ProjectId == request.ProjectId)
            .Select(f => new
            {
                f.Id,
                f.Title,
                f.Description,
                f.EpicLink,
                f.PriorityId,
                f.CompletedAt,
                f.CreatedAt,
                StatusName = f.CurrentStatus.StatusName,
                CreatedBy  = f.CreatedByUser.FullName,
                Release    = f.Release == null ? null : new { f.Release.Id, f.Release.Name }
            })
            .FirstOrDefaultAsync(ct);

        if (feature == null) return null;

        //List<int>? activeStageIds = null;
        //if (feature.Release != null)
        //{
        //    activeStageIds = await _db.ReleaseStages
        //        .Where(rs => rs.ReleaseId == feature.Release.Id)
        //        .Select(rs => rs.StageId)
        //        .ToListAsync(ct);
        //}

        //var stagesQuery = _db.Stages.AsNoTracking();
        //if (activeStageIds != null)
        //    stagesQuery = stagesQuery.Where(s => activeStageIds.Contains(s.Id));

        //var existingAssignments = await _db.FeatureAssignments
        //    .Where(fa => fa.FeatureId == request.FeatureId)
        //    .Select(fa => new
        //    {
        //        fa.StageId,
        //        fa.AssignedUserId,
        //        FullName = fa.AssignedUser.FullName,
        //        fa.CompletedAt 
        //    })
        //    .ToListAsync(ct);

        //var assignments = stagesQuery
        //    .OrderBy(s => s.Sequence)
        //    .AsEnumerable()
        //    .Select(stage =>
        //    {
        //        var assignment = existingAssignments.FirstOrDefault(fa => fa.StageId == stage.Id);

        //        return new FeatureAssignmentDto
        //        {
        //            StageId = stage.Id,
        //            StageName = stage.StageName,
        //            UserId = assignment?.AssignedUserId,
        //            UserName = assignment?.FullName,
        //            CompletedAt = assignment?.CompletedAt 
        //        };
        //    })
        //    .ToList();

        var releaseId = feature.Release?.Id;

        var stagesQuery = _db.Stages.AsNoTracking();

        if (releaseId.HasValue)
        {
            stagesQuery = stagesQuery.Where(stage => _db.ReleaseStages.Any(rs => rs.ReleaseId == releaseId.Value && rs.StageId == stage.Id));
        }

        //get assignments + stages in one query, insted of many queries for ReleaseStages/Stages/FeatureAssignments.
        var assignments = await (
            from stage in stagesQuery
            join assignment in _db.FeatureAssignments
                    .AsNoTracking()
                    .Where(fa => fa.FeatureId == request.FeatureId)
                on stage.Id equals assignment.StageId into assignmentGroup
            from assignment in assignmentGroup.DefaultIfEmpty()
            orderby stage.Sequence
            select new FeatureAssignmentDto
            {
                StageId = stage.Id,
                StageName = stage.StageName,
                UserId = assignment == null ? (int?)null : assignment.AssignedUserId,
                UserName = assignment == null ? null : assignment.AssignedUser.FullName,
                Estimation = assignment == null ? null : assignment.Estimation,
                EstimationUnitName = assignment == null ? null : assignment.EstimationUnit!.Name,
                StartedAt = assignment == null ? null : assignment.StartedAt,
                CompletedAt = assignment == null ? null : assignment.CompletedAt
            }
        ).ToListAsync(ct);

        var leadStages = await _db.ProjectLeads
            .AsNoTracking()
            .Where(pl => pl.ProjectId == request.ProjectId && pl.UserId == currentUserId)
            .Select(pl => pl.LeadRole.StageId)
            .ToListAsync(ct);

        return new FeatureDetailOutput
        {
            Id = feature.Id,
            Title = feature.Title,
            Description = feature.Description,
            EpicLink = feature.EpicLink,
            PriorityId = feature.PriorityId,
            CompletedAt = feature.CompletedAt,
            Status = feature.StatusName,
            Release = feature.Release == null ? null : new FeatureReleaseDto
            {
                Id = feature.Release.Id,
                Name = feature.Release.Name
            },
            CreatedBy = feature.CreatedBy,
            CreatedAt = feature.CreatedAt,
            Assignments = assignments,
            Permissions = new FeaturePermissionOutput
            {
                IsPM = isPM,
                LeadStageIds = leadStages
            }
        };
    }
}
