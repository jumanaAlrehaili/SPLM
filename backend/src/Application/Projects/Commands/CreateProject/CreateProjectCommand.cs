using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Commands.CreateProject;

public record CreateProjectCommand(CreateProjectInput Input) : IRequest<int>;
