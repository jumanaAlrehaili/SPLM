using Application.Shared.DTOs.Feature;
using MediatR;

namespace Application.Features.Commands.UpdateFeature;

public record UpdateFeatureCommand(int ProjectId, int FeatureId, UpdateFeatureInput Input) : IRequest<FeatureDetailOutput>;
