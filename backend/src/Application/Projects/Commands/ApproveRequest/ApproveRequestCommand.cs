using MediatR;

namespace Application.Projects.Commands.ApproveRequest;

public record ApproveRequestCommand(int RequestId) : IRequest;
