using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Queries.GetAllProjects;

public record GetAllProjectsQuery(int Page, int PageSize) : IRequest<PagedResult<ProjectOutput>>;
