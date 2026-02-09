namespace WroclawTheatreTickets.Web;

using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

public static class RateLimitingConfiguration
{
    public const string PublicEndpointsPolicy = "PublicEndpoints";
    public const string AuthenticatedEndpointsPolicy = "AuthenticatedEndpoints";
    public const string AdminEndpointsPolicy = "AdminEndpoints";

    // Configuration constants to avoid duplication
    private const int PublicPermitLimit = 200;
    private const int AuthenticatedPermitLimit = 50;
    private const int AdminPermitLimit = 1000;
    private const int WindowMinutes = 1;
    private const int PublicQueueLimit = 0;
    private const int AuthenticatedQueueLimit = 2;
    private const int AdminQueueLimit = 0;

    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Public endpoints: Rate limited by IP address
            options.AddPolicy(PublicEndpointsPolicy, httpContext =>
            {
                var ipAddress = GetIpAddress(httpContext);
                return RateLimitPartition.GetFixedWindowLimiter(ipAddress, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = PublicPermitLimit,
                    Window = TimeSpan.FromMinutes(WindowMinutes),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = PublicQueueLimit
                });
            });

            // Authenticated endpoints: Rate limited by user ID
            options.AddPolicy(AuthenticatedEndpointsPolicy, httpContext =>
            {
                var userId = GetUserId(httpContext) ?? GetIpAddress(httpContext);
                return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = AuthenticatedPermitLimit,
                    Window = TimeSpan.FromMinutes(WindowMinutes),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = AuthenticatedQueueLimit
                });
            });

            // Admin endpoints: Rate limited by user ID
            options.AddPolicy(AdminEndpointsPolicy, httpContext =>
            {
                var userId = GetUserId(httpContext) ?? GetIpAddress(httpContext);
                return RateLimitPartition.GetFixedWindowLimiter(userId, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = AdminPermitLimit,
                    Window = TimeSpan.FromMinutes(WindowMinutes),
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = AdminQueueLimit
                });
            });

            options.OnRejected = async (context, token) =>
            {
                context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                
                if (context.Lease.TryGetMetadata(MetadataName.RetryAfter, out var retryAfter))
                {
                    await context.HttpContext.Response.WriteAsync(
                        $"Too many requests. Please try again after {retryAfter.TotalSeconds} seconds.", token);
                }
                else
                {
                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Please try again later.", token);
                }
            };
        });

        return services;
    }

    private static string GetIpAddress(HttpContext httpContext)
    {
        // Check for forwarded IP first (for proxies/load balancers)
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            return forwardedFor.Split(',')[0].Trim();
        }

        // Fall back to remote IP
        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static string? GetUserId(HttpContext httpContext)
    {
        var userId = httpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return userId;
    }
}
