namespace WroclawTheatreTickets.Application.Contracts.Services;

public interface INotificationService
{
    Task SendPushNotificationAsync(Guid userId, string title, string message);
    Task SendEventReminderNotificationAsync(Guid userId, Guid showId, string showTitle);
    Task SendWeeklyDigestNotificationAsync(Guid userId, object shows);
}
