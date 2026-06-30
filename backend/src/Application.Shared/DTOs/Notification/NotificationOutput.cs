namespace Application.Shared.DTOs.Notification;

public record NotificationOutput
{
    public int Id { get; init; }
    public string Message { get; init; } = string.Empty;
    public bool IsRead { get; init; }
    public DateTime CreatedAt { get; init; }
    public int? FeatureId { get; init; }
    public string? FeatureTitle { get; init; }
}
