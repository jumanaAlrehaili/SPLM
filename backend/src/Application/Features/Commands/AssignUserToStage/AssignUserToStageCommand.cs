using MediatR;

namespace Application.Features.Commands.AssignUserToStage;

public record AssignUserToStageCommand(int ProjectId, int FeatureId, int StageId, int AssignedUserId) : IRequest;
