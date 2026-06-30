using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Queries.GetProjectById;

public record GetProjectByIdQuery(int ProjectId) : IRequest<ProjectOutput?>;
