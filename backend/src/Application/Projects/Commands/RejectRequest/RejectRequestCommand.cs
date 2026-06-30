using MediatR;

namespace Application.Projects.Commands.RejectRequest;

public record RejectRequestCommand(int RequestId) : IRequest;
