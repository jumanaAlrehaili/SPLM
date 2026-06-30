using Application.Shared.Interfaces;
using Domain.Entities.Projects;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands.SetProjectLead;

public class SetProjectLeadHandler : IRequestHandler<SetProjectLeadCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public SetProjectLeadHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(SetProjectLeadCommand request, CancellationToken ct)
    {
        var isManager = await _db.ProjectResources
            .AnyAsync(res => res.ProjectId == request.ProjectId
                          && res.UserId == _currentUser.UserId
                          && res.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isManager)
            throw new UnauthorizedAccessException("Only the Project Manager can assign leaders.");

        var isResource = await _db.ProjectResources
            .AnyAsync(pr => pr.ProjectId == request.ProjectId && pr.UserId == request.Input.UserId, ct);

        if (!isResource)
            throw new KeyNotFoundException("The selected user is not a resource in this project. Add them to the project first.");

        var roleExists = await _db.LeadRoles.AnyAsync(r => r.Id == request.Input.LeadRoleId, ct);

        if (!roleExists)
            throw new KeyNotFoundException("Invalid lead role specified.");

        var existingLead = await _db.ProjectLeads
            .FirstOrDefaultAsync(pl => pl.ProjectId == request.ProjectId && pl.LeadRoleId == request.Input.LeadRoleId, ct);

        var now = DateTime.UtcNow;

        if (existingLead != null)
        {
            existingLead.UserId          = request.Input.UserId;
            existingLead.UpdatedAt       = now;
            existingLead.UpdatedByUserId = _currentUser.UserId;
        }
        else
        {
            _db.ProjectLeads.Add(new ProjectLead
            {
                ProjectId       = request.ProjectId,
                LeadRoleId      = request.Input.LeadRoleId,
                UserId          = request.Input.UserId,
                CreatedAt       = now,
                CreatedByUserId = _currentUser.UserId
            });
        }

        await _db.SaveChangesAsync(ct);
    }
}
