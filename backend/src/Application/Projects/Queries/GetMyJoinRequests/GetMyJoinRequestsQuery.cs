using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Queries.GetMyJoinRequests;

public record GetMyJoinRequestsQuery : IRequest<IEnumerable<JoinRequestOutput>>;
