using Application.Shared.DTOs.Feature;
using MediatR;

namespace Application.Features.Queries.GetAvailableResourcesForStage;

public record GetAvailableResourcesForStageQuery(int ProjectId, int StageId)
    : IRequest<IEnumerable<AvailableStageResourceDto>>;
