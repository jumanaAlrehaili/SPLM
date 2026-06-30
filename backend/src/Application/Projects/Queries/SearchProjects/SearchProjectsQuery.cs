using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Queries.SearchProjects;

public record SearchProjectsQuery(string? Name, DateTime? CreatedFrom, DateTime? CreatedTo, int Page, int PageSize)
    : IRequest<PagedResult<ProjectOutput>>;
