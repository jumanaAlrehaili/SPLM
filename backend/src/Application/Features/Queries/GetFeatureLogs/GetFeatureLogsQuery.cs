using Application.Shared.DTOs.Feature;
using MediatR;

namespace Application.Features.Queries.GetFeatureLogs;

public record GetFeatureLogsQuery(int ProjectId, int FeatureId) : IRequest<IEnumerable<FeatureStageLogOutput>>;
