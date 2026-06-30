using Application.Features.Queries.GetFeatureById;
using Application.Shared.DTOs.Feature;
using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commands.UpdateFeature;

public class UpdateFeatureHandler : IRequestHandler<UpdateFeatureCommand, FeatureDetailOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public UpdateFeatureHandler(IAppDbContext db, ICurrentUserService currentUser, IMediator mediator)
    {
        _db = db;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<FeatureDetailOutput> Handle(UpdateFeatureCommand request, CancellationToken ct)
    {
        var isBA = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.BusinessAnalyst.Id, ct);

        if (!isBA)
            throw new UnauthorizedAccessException("Only a Business Analyst of this project can perform this action.");

        var feature = await _db.Features
            .FirstOrDefaultAsync(f => f.Id == request.FeatureId && f.ProjectId == request.ProjectId, ct)
            ?? throw new KeyNotFoundException("Feature not found.");

        feature.Title           = request.Input.Title;
        feature.Description     = request.Input.Description;
        feature.EpicLink        = request.Input.EpicLink;
        feature.PriorityId      = request.Input.Priority;
        feature.UpdatedAt       = DateTime.UtcNow;
        feature.UpdatedByUserId = _currentUser.UserId;

        await _db.SaveChangesAsync(ct);

        return await _mediator.Send(new GetFeatureByIdQuery(request.ProjectId, feature.Id), ct)
            ?? throw new Exception("Failed to retrieve updated feature.");
    }
}
