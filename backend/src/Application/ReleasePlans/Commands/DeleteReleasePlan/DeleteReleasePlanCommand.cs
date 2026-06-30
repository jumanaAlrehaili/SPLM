using MediatR;

namespace Application.ReleasePlans.Commands.DeleteReleasePlan;

public record DeleteReleasePlanCommand(int ProjectId, int PlanId) : IRequest;