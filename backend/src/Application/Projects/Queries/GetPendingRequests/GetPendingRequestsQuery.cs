using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Queries.GetPendingRequests;

public record GetPendingRequestsQuery : IRequest<IEnumerable<JoinRequestOutput>>;
