using Application.Shared.DTOs.ReleasePlan;
using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ReleasePlans.Commands.UpdateReleasePlan;

public class UpdateReleasePlanHandler : IRequestHandler<UpdateReleasePlanCommand, ReleasePlanOutput>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UpdateReleasePlanHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<ReleasePlanOutput> Handle(UpdateReleasePlanCommand request, CancellationToken ct)
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

        var plan = await _db.ReleasePlans
            .FirstOrDefaultAsync(p => p.Id == request.PlanId && p.ProjectId == request.ProjectId, ct)
            ?? throw new KeyNotFoundException("Release plan not found.");

        var nameExists = await _db.ReleasePlans
          .AnyAsync(p =>
              p.ProjectId == request.ProjectId &&
              p.Id != request.PlanId &&
              p.Name == planName,
              ct);

        if (nameExists)
            throw new InvalidOperationException(
                $"A release plan with the name '{planName}' already exists in this project.");

        plan.Name = planName;
        plan.Description = request.Input.Description;
        plan.UpdatedAt = DateTime.UtcNow;
        plan.UpdatedByUserId = _currentUser.UserId;

        await _db.SaveChangesAsync(ct);

        return await _db.ReleasePlans
            .AsNoTracking()
            .Where(p => p.Id == plan.Id)
            .Select(p => new ReleasePlanOutput
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ReleaseCount = p.Releases.Count(),
                CreatedBy = p.CreatedByUser.FullName,
                CreatedAt = p.CreatedAt
            })
            .FirstAsync(ct);
    }
}