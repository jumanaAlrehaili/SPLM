using Application.Shared.DTOs.Project;
using MediatR;

namespace Application.Projects.Commands.SubmitJoinRequest;

public record SubmitJoinRequestCommand(SubmitJoinRequestInput Input) : IRequest<int>;
