using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Feature;
using MediatR;

namespace Application.Features.Queries.SearchFeatures;

public record SearchFeaturesQuery(int ProjectId, string? Name, int? Priority, int? StatusId, int Page, int PageSize)
    : IRequest<PagedResult<FeatureOutput>>;
