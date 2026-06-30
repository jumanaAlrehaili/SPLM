using MediatR;

namespace Application.Notifications.Commands.MarkAsRead;

public record MarkAsReadCommand(int NotificationId) : IRequest;
