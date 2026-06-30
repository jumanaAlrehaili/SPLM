using MediatR;

namespace Application.ReleasePlans.Commands.DeleteRelease;

public record DeleteReleaseCommand(int ProjectId, int PlanId, int ReleaseId) : IRequest;