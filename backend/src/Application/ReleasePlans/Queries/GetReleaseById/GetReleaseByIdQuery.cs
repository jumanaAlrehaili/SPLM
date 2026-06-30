using Application.Shared.DTOs.ReleasePlan;
using MediatR;

namespace Application.ReleasePlans.Queries.GetReleaseById;

public record GetReleaseByIdQuery(int ProjectId, int PlanId, int ReleaseId) : IRequest<ReleaseDetailOutput?>;