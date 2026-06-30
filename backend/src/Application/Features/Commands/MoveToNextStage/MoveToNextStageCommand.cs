using MediatR;

namespace Application.Features.Commands.MoveToNextStage;

public record MoveToNextStageCommand(int ProjectId, int FeatureId, int CurrentStageId) : IRequest;
