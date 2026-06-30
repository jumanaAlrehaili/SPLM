using Application.ReleasePlans.Queries.GetReleaseById;
using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using Domain.Entities.Releases;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ReleasePlans.Commands.CreateRelease;

public class CreateReleaseHandler : IRequestHandler<CreateReleaseCommand, ReleaseDetailOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator; 

    public CreateReleaseHandler(IAppDbContext db, ICurrentUserService currentUser, IMediator mediator)
    {
        _db = db;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<ReleaseDetailOutput> Handle(CreateReleaseCommand request, CancellationToken ct)
    {
        var releaseName = request.Input.Name.Trim();

        if (string.IsNullOrWhiteSpace(releaseName))
            throw new InvalidOperationException("Release name is required.");

        var isPM = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isPM)
            throw new UnauthorizedAccessException("Only the Project Manager can perform this action.");

        var belongs = await _db.ReleasePlans
            .AnyAsync(p => p.Id == request.PlanId && p.ProjectId == request.ProjectId, ct);

        if (!belongs)
            throw new KeyNotFoundException("Release plan not found in this project.");

        var nameExists = await _db.Releases
           .AnyAsync(r => r.ReleasePlanId == request.PlanId && r.Name == releaseName, ct);

        if (nameExists)
            throw new InvalidOperationException($"A release with the name '{releaseName}' already exists in this plan.");

        var release = new Release
        {
            ReleasePlanId = request.PlanId,
            Name = releaseName,
            Description = request.Input.Description,
            StatusId = FeatureStatusLookup.New.Id,  //from statuses table
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _currentUser.UserId
        };

        _db.Releases.Add(release);
        await _db.SaveChangesAsync(ct);

      

        return await _mediator.Send(new GetReleaseByIdQuery(request.ProjectId, request.PlanId, release.Id), ct)
            ?? throw new Exception("Failed to retrieve created release.");
    }
}