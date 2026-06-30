using Application.Shared.DTOs.Notification;
using MediatR;

namespace Application.Notifications.Queries.GetMyNotifications;

public record GetMyNotificationsQuery(bool UnreadOnly) : IRequest<IEnumerable<NotificationOutput>>;
