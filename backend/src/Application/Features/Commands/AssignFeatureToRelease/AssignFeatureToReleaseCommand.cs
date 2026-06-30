using Application.Shared.DTOs.Feature;
using MediatR;

namespace Application.Features.Commands.AssignFeatureToRelease;

public record AssignFeatureToReleaseCommand(int ProjectId, int FeatureId, int? ReleaseId) : IRequest<FeatureDetailOutput>;
