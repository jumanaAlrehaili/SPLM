using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Features.Commands.DeleteFeature;

public class DeleteFeatureHandler : IRequestHandler<DeleteFeatureCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteFeatureHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteFeatureCommand request, CancellationToken ct)
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

        _db.Features.Remove(feature);
        await _db.SaveChangesAsync(ct);
    }
}
