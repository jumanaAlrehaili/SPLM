using Domain.Entities.Features;
using Domain.IdentityEntities;

namespace Domain.Entities.Notifications;

public class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public virtual ApplicationUser User { get; set; } = null!;

    public int? FeatureId { get; set; }
    public virtual Feature? Feature { get; set; }

    public string Message { get; set; } = null!;
    public bool IsRead { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
