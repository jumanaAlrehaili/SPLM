using Application.Shared.DTOs.Feature;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Queries.GetFeatureLogs;

public class GetFeatureLogsHandler : IRequestHandler<GetFeatureLogsQuery, IEnumerable<FeatureStageLogOutput>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetFeatureLogsHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<FeatureStageLogOutput>> Handle(GetFeatureLogsQuery request, CancellationToken ct)
    {
        var isMember = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId && r.UserId == _currentUser.UserId, ct);

        if (!isMember)
            throw new UnauthorizedAccessException("You are not a member of this project.");

        return await _db.FeatureStageLogs
            .AsNoTracking()
            .Where(l => l.FeatureId == request.FeatureId && l.Feature.ProjectId == request.ProjectId)
            .OrderByDescending(l => l.Timestamp)
            .Select(l => new FeatureStageLogOutput
            {
                Id        = l.Id,
                Action    = l.Action,
                Comment   = l.Comment,
                Timestamp = l.Timestamp,
                UserId    = l.UserId,
                UserName  = l.User.FullName,
                StageName = l.Stage.StageName
            })
            .ToListAsync(ct);
    }
}
