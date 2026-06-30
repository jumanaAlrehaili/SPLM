using Application.Shared.DTOs.Feature;
using MediatR;

namespace Application.Features.Queries.GetFeatureById;

public record GetFeatureByIdQuery(int ProjectId, int FeatureId) : IRequest<FeatureDetailOutput?>;
