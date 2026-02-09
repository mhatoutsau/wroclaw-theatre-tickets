namespace WroclawTheatreTickets.Infrastructure.Configuration;

/// <summary>
/// Configuration for theatre API access (Teatr Polski).
/// Typically bound from appsettings.json under "TheatreApis:TeatrPolski"
/// </summary>
public class TheatreApiConfiguration
{
    /// <summary>
    /// Gets the API endpoint URL for fetching theatre repertoire
    /// </summary>
    public string Url { get; set; } = string.Empty;

    /// <summary>
    /// Gets the HTTP timeout in seconds for API requests
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;
}
