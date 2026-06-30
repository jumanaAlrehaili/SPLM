using Application.Shared.DTOs.Lookup;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Lookups.Queries;

// ── MediatR Requests (Queries) ──────────────────────────────────
public record GetStagesQuery : IRequest<IEnumerable<StageLookupDto>>;
public record GetStatusesQuery : IRequest<IEnumerable<StatusLookupDto>>;
public record GetReleaseStageStatusesQuery : IRequest<IEnumerable<StatusLookupDto>>;
public record GetRolesQuery : IRequest<IEnumerable<RoleLookupDto>>;
public record GetFeaturePrioritiesQuery : IRequest<IEnumerable<FeaturePriorityDto>>;
public record GetRequestStatusesQuery : IRequest<IEnumerable<RequestStatusDto>>;
public record GetDurationUnitsQuery : IRequest<IEnumerable<DurationUnitDto>>;
public record GetProjectMembershipStatusesQuery : IRequest<IEnumerable<ProjectMembershipStatusDto>>;
public record GetReleaseStageChangeTypesQuery : IRequest<IEnumerable<ReleaseStageChangeTypeDto>>;
public record GetEstimationUnitsQuery : IRequest<IEnumerable<EstimationUnitDto>>;


// ── Lookups Handler ──────────────────────────────────────
public class LookupsHandler :
    IRequestHandler<GetStagesQuery, IEnumerable<StageLookupDto>>,
    IRequestHandler<GetStatusesQuery, IEnumerable<StatusLookupDto>>,
    IRequestHandler<GetReleaseStageStatusesQuery, IEnumerable<StatusLookupDto>>,
    IRequestHandler<GetRolesQuery, IEnumerable<RoleLookupDto>>,
    IRequestHandler<GetFeaturePrioritiesQuery, IEnumerable<FeaturePriorityDto>>,
    IRequestHandler<GetRequestStatusesQuery, IEnumerable<RequestStatusDto>>,
    IRequestHandler<GetDurationUnitsQuery, IEnumerable<DurationUnitDto>>,
    IRequestHandler<GetProjectMembershipStatusesQuery, IEnumerable<ProjectMembershipStatusDto>>,
    IRequestHandler<GetReleaseStageChangeTypesQuery, IEnumerable<ReleaseStageChangeTypeDto>>,
    IRequestHandler<GetEstimationUnitsQuery, IEnumerable<EstimationUnitDto>>
{
    private readonly IAppDbContext _db;

    public LookupsHandler(IAppDbContext db)
    {
        _db = db;
    }

    public async Task<IEnumerable<StageLookupDto>> Handle(GetStagesQuery request, CancellationToken ct)
    {
        return await _db.Stages.AsNoTracking().OrderBy(s => s.Sequence)
            .Select(s => new StageLookupDto(s.Id, s.StageName, s.Sequence, s.IsDefault)).ToListAsync(ct);
    }

    public async Task<IEnumerable<StatusLookupDto>> Handle(GetStatusesQuery request, CancellationToken ct)
    {
        return await _db.Statuses.AsNoTracking()
            .Select(s => new StatusLookupDto(s.Id, s.StatusName)).ToListAsync(ct);
    }

    public async Task<IEnumerable<StatusLookupDto>> Handle(GetReleaseStageStatusesQuery request, CancellationToken ct)
    {
        return await _db.ReleaseStageStatuses.AsNoTracking()
            .Select(s => new StatusLookupDto(s.Id, s.StatusName)).ToListAsync(ct);
    }

    public async Task<IEnumerable<RoleLookupDto>> Handle(GetRolesQuery request, CancellationToken ct)
    {
        return await _db.Roles.AsNoTracking()
            .Select(r => new RoleLookupDto(r.Id, r.Name ?? "")).ToListAsync(ct);
    }

    public async Task<IEnumerable<FeaturePriorityDto>> Handle(GetFeaturePrioritiesQuery request, CancellationToken ct)
    {
        return await _db.FeaturePriorities.AsNoTracking()
            .Select(x => new FeaturePriorityDto(x.Id, x.Name)).ToListAsync(ct);
    }

    public async Task<IEnumerable<RequestStatusDto>> Handle(GetRequestStatusesQuery request, CancellationToken ct)
    {
        return await _db.RequestStatuses.AsNoTracking()
            .Select(x => new RequestStatusDto(x.Id, x.Name)).ToListAsync(ct);
    }

    public async Task<IEnumerable<DurationUnitDto>> Handle(GetDurationUnitsQuery request, CancellationToken ct)
    {
        return await _db.DurationUnits.AsNoTracking()
            .Select(x => new DurationUnitDto(x.Id, x.Name)).ToListAsync(ct);
    }

    public async Task<IEnumerable<ProjectMembershipStatusDto>> Handle(GetProjectMembershipStatusesQuery request, CancellationToken ct)
    {
        return await _db.ProjectMembershipStatuses.AsNoTracking()
            .Select(x => new ProjectMembershipStatusDto(x.Id, x.Name)).ToListAsync(ct);
    }

    public async Task<IEnumerable<ReleaseStageChangeTypeDto>> Handle(GetReleaseStageChangeTypesQuery request, CancellationToken ct)
    {
        return await _db.ReleaseStageChangeTypes.AsNoTracking()
            .Select(x => new ReleaseStageChangeTypeDto(x.Id, x.Name)).ToListAsync(ct);
    }
    public async Task<IEnumerable<EstimationUnitDto>> Handle(GetEstimationUnitsQuery request, CancellationToken ct)
    {
        return await _db.EstimationUnits.AsNoTracking()
            .Select(x => new EstimationUnitDto(x.Id, x.Name))
            .ToListAsync(ct);
    }
}