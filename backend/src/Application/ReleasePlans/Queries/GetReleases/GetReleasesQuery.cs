using Application.Shared.DTOs.ReleasePlan;
using MediatR;
using System.Collections.Generic;

namespace Application.ReleasePlans.Queries.GetReleases;

public record GetReleasesQuery(int ProjectId, int PlanId) : IRequest<IEnumerable<ReleaseOutput>>;