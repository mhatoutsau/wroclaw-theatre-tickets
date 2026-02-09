namespace WroclawTheatreTickets.Web;

using System.Security.Claims;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

public static class RateLimitingConfiguration
{
    public const string PublicEndpointsPolicy = "PublicEndpoints";
    public const string AuthenticatedEndpointsPolicy = "AuthenticatedEndpoints";
    public const string AdminEndpointsPolicy = "AdminEndpoints";

    public static IServiceCollection AddRateLimitingPolicies(this IServiceCollection services)
    {
        services.AddRateLimiter(options =>
        {
            // Public endpoints: 200 req/min per IP, no queue (fail fast)
            options.AddFixedWindowLimiter(PublicEndpointsPolicy, opt =>
            {
                opt.PermitLimit = 200;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0; // No queueing - fail fast
            }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Authenticated endpoints: 50 req/min per user, queue of 2
            options.AddFixedWindowLimiter(AuthenticatedEndpointsPolicy, opt =>
            {
                opt.PermitLimit = 50;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 2; // Allow brief queueing
            }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Admin endpoints: 1000 req/min per user
            options.AddFixedWindowLimiter(AdminEndpointsPolicy, opt =>
            {
                opt.PermitLimit = 1000;
                opt.Window = TimeSpan.FromMinutes(1);
                opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                opt.QueueLimit = 0;
            }).RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            // Set partition key resolver for all policies
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            {
                // Determine which policy to use based on the endpoint
                var endpoint = httpContext.GetEndpoint();
                var rateLimitPolicy = endpoint?.Metadata.GetMetadata<EnableRateLimitingAttribute>()?.PolicyName;

                if (string.IsNullOrEmpty(rateLimitPolicy))
                {
                    return RateLimitPartition.GetNoLimiter<string>("NoLimit");
                }

                // Get partition key based on policy
                var partitionKey = GetPartitionKey(httpContext, rateLimitPolicy);

                return rateLimitPolicy switch
                {
                    PublicEndpointsPolicy => RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 200,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }),
                    AuthenticatedEndpointsPolicy => RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 50,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 2
                    }),
                    AdminEndpointsPolicy => RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 1000,
                        Window = TimeSpan.FromMinutes(1),
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 0
                    }),
                    _ => RateLimitPartition.GetNoLimiter<string>("NoLimit")
                };
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

    private static string GetPartitionKey(HttpContext httpContext, string policyName)
    {
        return policyName switch
        {
            PublicEndpointsPolicy => GetIpAddress(httpContext),
            AuthenticatedEndpointsPolicy => GetUserId(httpContext) ?? GetIpAddress(httpContext),
            AdminEndpointsPolicy => GetUserId(httpContext) ?? GetIpAddress(httpContext),
            _ => "unknown"
        };
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
