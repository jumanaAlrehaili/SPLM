using Application.Shared.DTOs.ReleasePlan;
using MediatR;

namespace Application.ReleasePlans.Queries.GetReleasePlanById;

public record GetReleasePlanByIdQuery(int ProjectId, int PlanId) : IRequest<ReleasePlanDetailOutput?>;