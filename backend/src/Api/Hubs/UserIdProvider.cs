using Microsoft.AspNetCore.SignalR;

namespace Api.Hubs;

public class UserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
        => connection.User?.FindFirst("userId")?.Value;
}
