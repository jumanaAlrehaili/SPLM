using Application.Shared.DTOs.Notification;
using Application.Shared.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Notifications.Queries.GetMyNotifications;

public class GetMyNotificationsHandler : IRequestHandler<GetMyNotificationsQuery, IEnumerable<NotificationOutput>>
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public GetMyNotificationsHandler(IAppDbContext db, ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<IEnumerable<NotificationOutput>> Handle(GetMyNotificationsQuery request, CancellationToken ct)
    {
        return await _db.Notifications
            .AsNoTracking()
            .Where(n => n.UserId == _currentUser.UserId)
            .Where(n => !request.UnreadOnly || !n.IsRead)
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new NotificationOutput
            {
                Id           = n.Id,
                Message      = n.Message,
                IsRead       = n.IsRead,
                CreatedAt    = n.CreatedAt,
                FeatureId    = n.FeatureId,
                FeatureTitle = n.Feature != null ? n.Feature.Title : null
            })
            .ToListAsync(ct);
    }
}
