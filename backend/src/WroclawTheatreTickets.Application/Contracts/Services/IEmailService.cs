namespace WroclawTheatreTickets.Application.Contracts.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendWeeklyDigestAsync(string to, object events);
    Task SendEventReminderAsync(string to, string eventTitle, DateTime eventDateTime);
    Task SendVerificationEmailAsync(string to, string verificationUrl);
    Task SendPasswordResetEmailAsync(string to, string resetUrl);
}
