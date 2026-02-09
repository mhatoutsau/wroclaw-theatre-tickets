namespace WroclawTheatreTickets.Application.Contracts.Services;

public interface IAuthenticationService
{
    Task<string> HashPasswordAsync(string password);
    Task<bool> VerifyPasswordAsync(string password, string hash);
    Task<string> GenerateJwtTokenAsync(Guid userId, string email, string role);
    Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(Guid userId, string email, string role);
}

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, bool isHtml = true);
    Task SendWeeklyDigestAsync(string to, object events);
    Task SendEventReminderAsync(string to, string eventTitle, DateTime eventDateTime);
    Task SendVerificationEmailAsync(string to, string verificationUrl);
    Task SendPasswordResetEmailAsync(string to, string resetUrl);
}

public interface INotificationService
{
    Task SendPushNotificationAsync(Guid userId, string title, string message);
    Task SendEventReminderNotificationAsync(Guid userId, Guid showId, string showTitle);
    Task SendWeeklyDigestNotificationAsync(Guid userId, object shows);
}

public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
}
