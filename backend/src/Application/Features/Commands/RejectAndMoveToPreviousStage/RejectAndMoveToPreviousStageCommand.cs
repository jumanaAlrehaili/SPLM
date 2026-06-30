using MediatR;

namespace Application.Features.Commands.RejectAndMoveToPreviousStage;

public record RejectAndMoveToPreviousStageCommand(int ProjectId, int FeatureId, int CurrentStageId, string RejectionComment) : IRequest;
