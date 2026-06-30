using Application.Shared.Interfaces;
using Domain.Entities.Projects;
using Domain.Lookups;
using MediatR;

namespace Application.Projects.Commands.CreateProject;

public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, int>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public CreateProjectHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<int> Handle(CreateProjectCommand request, CancellationToken ct)
    {
        if (request.Input.DurationUnit.HasValue && !DurationUnitsLookup.All.Any(d => d.Id == request.Input.DurationUnit.Value))
            throw new InvalidOperationException("Invalid duration unit.");

        var now = DateTime.UtcNow;

        var project = new Project
        {
            Name            = request.Input.Name,
            Description     = request.Input.Description,
            Budget          = request.Input.Budget,
            Duration        = request.Input.Duration,
            DurationUnitId  = request.Input.DurationUnit,
            CreatedAt       = now,
            CreatedByUserId = _currentUser.UserId
        };

        project.ProjectResources.Add(new ProjectResource
        {
            UserId          = _currentUser.UserId,
            RoleId          = RoleLookup.ProjectManager.Id,
            CreatedAt       = now,
            CreatedByUserId = _currentUser.UserId
        });

        _db.Projects.Add(project);
        await _db.SaveChangesAsync(ct);

        return project.Id;
    }
}
