using Application.Shared.DTOs.Common;
using Application.Shared.DTOs.Feature;
using MediatR;

namespace Application.Features.Queries.GetAllFeatures;

public record GetAllFeaturesQuery(int ProjectId, int Page, int PageSize) : IRequest<PagedResult<FeatureOutput>>;
