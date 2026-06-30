using MediatR;

namespace Application.Features.Commands.DeleteFeature;

public record DeleteFeatureCommand(int ProjectId, int FeatureId) : IRequest;
