using MediatR;

namespace Application.ReleaseStages.Commands.MoveReleaseStage;

public record MoveReleaseStageCommand(int ProjectId, int PlanId, int ReleaseId, int StageId, string? Notes) : IRequest;