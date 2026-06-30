using Application.Features.Queries.GetFeatureById;
using Application.Shared.DTOs.Feature;
using Application.Shared.Interfaces;
using Domain.Entities.Features;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commands.CreateFeature;

public class CreateFeatureHandler : IRequestHandler<CreateFeatureCommand, FeatureDetailOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CreateFeatureHandler(IAppDbContext db, ICurrentUserService currentUser, IMediator mediator)
    {
        _db = db;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<FeatureDetailOutput> Handle(CreateFeatureCommand request, CancellationToken ct)
    {
        var isBA = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.BusinessAnalyst.Id, ct);

        if (!isBA)
            throw new UnauthorizedAccessException("Only a Business Analyst of this project can perform this action.");

        var feature = new Feature
        {
            Title           = request.Input.Title,
            Description     = request.Input.Description,
            EpicLink        = request.Input.EpicLink,
            PriorityId      = request.Input.Priority,
            CurrentStatusId = FeatureStatusLookup.New.Id,
            ProjectId       = request.ProjectId,
            CreatedAt       = DateTime.UtcNow,
            CreatedByUserId = _currentUser.UserId
        };

        _db.Features.Add(feature);
        await _db.SaveChangesAsync(ct);

        return await _mediator.Send(new GetFeatureByIdQuery(request.ProjectId, feature.Id), ct)
            ?? throw new Exception("Failed to retrieve created feature.");
    }
}
