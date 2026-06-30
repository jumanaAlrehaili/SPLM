using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Queries.GetResources;

public record GetResourcesQuery(int? ProjectId = null, int Page = 1, int PageSize = 10) : IRequest<PagedResult<ResourceOutput>>;
