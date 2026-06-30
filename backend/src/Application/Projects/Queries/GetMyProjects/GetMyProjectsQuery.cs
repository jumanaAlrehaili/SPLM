using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Queries.GetMyProjects;

public record GetMyProjectsQuery(int Page, int PageSize) : IRequest<PagedResult<ProjectOutput>>;
