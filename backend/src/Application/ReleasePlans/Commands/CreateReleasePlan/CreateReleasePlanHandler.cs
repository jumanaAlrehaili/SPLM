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

namespace Application.ReleasePlans.Commands.CreateReleasePlan;

public class CreateReleasePlanHandler : IRequestHandler<CreateReleasePlanCommand, ReleasePlanOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateReleasePlanHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ReleasePlanOutput> Handle(CreateReleasePlanCommand request, CancellationToken ct)
    {
        var planName = request.Input.Name.Trim();

        if (string.IsNullOrWhiteSpace(planName))
            throw new InvalidOperationException("Release plan name is required.");

        var isPM = await _db.ProjectResources
            .AnyAsync(r => r.ProjectId == request.ProjectId
                        && r.UserId == _currentUser.UserId
                        && r.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isPM)
            throw new UnauthorizedAccessException("Only the Project Manager can perform this action.");

        var nameExists = await _db.ReleasePlans
            .AnyAsync(p =>
                p.ProjectId == request.ProjectId &&
                p.Name == planName,
                ct);

        if (nameExists)
            throw new InvalidOperationException($"A release plan with the name '{planName}' already exists in this project.");

        var plan = new ReleasePlan
        {
            ProjectId = request.ProjectId,
            Name = planName,
            Description = request.Input.Description,
            CreatedAt = DateTime.UtcNow,
            CreatedByUserId = _currentUser.UserId
        };

        _db.ReleasePlans.Add(plan);
        await _db.SaveChangesAsync(ct); 

        return await _db.ReleasePlans
            .AsNoTracking()
            .Where(p => p.Id == plan.Id)
            .Select(p => new ReleasePlanOutput
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ReleaseCount = 0,
                CreatedBy = p.CreatedByUser.FullName,
                CreatedAt = p.CreatedAt
            })
            .FirstAsync(ct);
    }
}