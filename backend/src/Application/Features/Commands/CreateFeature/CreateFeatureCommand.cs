using Application.Shared.DTOs.Feature;
using MediatR;

namespace Application.Features.Commands.CreateFeature;

public record CreateFeatureCommand(int ProjectId, CreateFeatureInput Input) : IRequest<FeatureDetailOutput>;
