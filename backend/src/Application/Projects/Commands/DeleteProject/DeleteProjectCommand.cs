using MediatR;

namespace Application.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(int ProjectId) : IRequest;
