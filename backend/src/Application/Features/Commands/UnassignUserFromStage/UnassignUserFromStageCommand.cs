using MediatR;

namespace Application.Features.Commands.UnassignUserFromStage;

public record UnassignUserFromStageCommand(int ProjectId, int FeatureId, int StageId) : IRequest;
