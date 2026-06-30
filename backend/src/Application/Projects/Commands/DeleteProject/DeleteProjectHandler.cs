using Application.Shared.Interfaces;
using Domain.Lookups;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Projects.Commands.DeleteProject;

public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeleteProjectHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task Handle(DeleteProjectCommand request, CancellationToken ct)
    {
        var project = await _db.Projects
            .FirstOrDefaultAsync(p => p.Id == request.ProjectId, ct)
            ?? throw new KeyNotFoundException("Project not found.");

        var isManager = await _db.ProjectResources
            .AnyAsync(res => res.ProjectId == request.ProjectId
                          && res.UserId == _currentUser.UserId
                          && res.RoleId == RoleLookup.ProjectManager.Id, ct);

        if (!isManager)
            throw new UnauthorizedAccessException("Only a Project Manager can delete this project.");

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync(ct);
    }
}
