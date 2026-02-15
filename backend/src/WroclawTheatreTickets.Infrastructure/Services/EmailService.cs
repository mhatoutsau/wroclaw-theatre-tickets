namespace WroclawTheatreTickets.Infrastructure.Services;

using WroclawTheatreTickets.Application.Contracts.Services;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;

public class EmailService : IEmailService
{
    private readonly string _smtpServer;
    private readonly int _smtpPort;
    private readonly string _emailFrom;
    private readonly string _emailPassword;

    public EmailService(IConfiguration configuration)
    {
        _smtpServer = configuration["Email:SmtpServer"] ?? "localhost";
        _smtpPort = int.Parse(configuration["Email:SmtpPort"] ?? "587");
        _emailFrom = configuration["Email:From"] ?? "noreply@wroclawtheatretickets.pl";
        _emailPassword = configuration["Email:Password"] ?? "";
    }

    public async Task SendEmailAsync(string to, string subject, string body, bool isHtml = true)
    {
        try
        {
            using (var client = new SmtpClient(_smtpServer, _smtpPort))
            {
                client.EnableSsl = true;
                if (!string.IsNullOrEmpty(_emailPassword))
                {
                    client.Credentials = new NetworkCredential(_emailFrom, _emailPassword);
                }

                var mailMessage = new MailMessage(_emailFrom, to)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isHtml
                };

                await client.SendMailAsync(mailMessage);
            }
        }
        catch (Exception ex)
        {
            // Log exception
            throw new Exception($"Failed to send email: {ex.Message}");
        }
    }

    public async Task SendWeeklyDigestAsync(string to, object events)
    {
        var subject = "Your Weekly Theatre Events Digest";
        var body = GenerateDigestHtml(events);
        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendEventReminderAsync(string to, string eventTitle, DateTime eventDateTime)
    {
        var subject = $"Reminder: {eventTitle} is coming up!";
        var body = $@"
            <h2>Upcoming Event Reminder</h2>
            <p><strong>{eventTitle}</strong></p>
            <p>This event starts on <strong>{eventDateTime:MMMM dd, yyyy at HH:mm}</strong></p>
            <p><a href='https://wroclawtheatretickets.pl'>View on Wrocław Theatre Tickets</a></p>
        ";
        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendVerificationEmailAsync(string to, string verificationUrl)
    {
        var subject = "Verify Your Email Address";
        var body = $@"
            <h2>Email Verification</h2>
            <p>Please verify your email address by clicking the link below:</p>
            <p><a href='{verificationUrl}'>Verify Email</a></p>
        ";
        await SendEmailAsync(to, subject, body, true);
    }

    public async Task SendPasswordResetEmailAsync(string to, string resetUrl)
    {
        var subject = "Reset Your Password";
        var body = $@"
            <h2>Password Reset Request</h2>
            <p>Click the link below to reset your password:</p>
            <p><a href='{resetUrl}'>Reset Password</a></p>
        ";
        await SendEmailAsync(to, subject, body, true);
    }

    private string GenerateDigestHtml(object events)
    {
        return @"
            <h2>Your Weekly Theatre Events Digest</h2>
            <p>Here are the theatre events you might be interested in:</p>
            <!-- Events will be rendered here -->
            <hr />
            <p>Visit <a href='https://wroclawtheatretickets.pl'>Wrocław Theatre Tickets</a> for more events.</p>
        ";
    }
}
