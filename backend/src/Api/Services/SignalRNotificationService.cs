using Api.Hubs;
using Application.Shared.DTOs.Notification;
using Application.Shared.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Api.Services;

public class SignalRNotificationService : IRealtimeNotificationService
{
    private readonly IHubContext<NotificationHub> _hubContext;

    public SignalRNotificationService(IHubContext<NotificationHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNotificationAsync(int userId, NotificationOutput notification)
    {
        await _hubContext.Clients
            .User(userId.ToString())
            .SendAsync("ReceiveNotification", notification);
    }
}
