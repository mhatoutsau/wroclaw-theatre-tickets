namespace WroclawTheatreTickets.Infrastructure.Services;

using WroclawTheatreTickets.Application.Contracts.Services;

public class NotificationService : INotificationService
{
    private readonly IEmailService _emailService;

    public NotificationService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendPushNotificationAsync(Guid userId, string title, string message)
    {
        // Implement push notification (Firebase, OneSignal, etc.)
        await Task.CompletedTask;
    }

    public async Task SendEventReminderNotificationAsync(Guid userId, Guid showId, string showTitle)
    {
        // Implement reminder notification
        await Task.CompletedTask;
    }

    public async Task SendWeeklyDigestNotificationAsync(Guid userId, object shows)
    {
        // Implement weekly digest notification
        await Task.CompletedTask;
    }
}
