using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Queries.SearchResources;

public record SearchResourcesQuery(string? SearchTerm, List<int>? ProjectId, List<int>? RoleId, int Page, int PageSize)
    : IRequest<PagedResult<ResourceOutput>>;
