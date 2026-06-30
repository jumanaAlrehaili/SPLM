using Application.Shared.DTOs.Notification;

namespace Application.Shared.Interfaces;

public interface IRealtimeNotificationService
{
    Task SendNotificationAsync(int userId, NotificationOutput notification);
}
