namespace WroclawTheatreTickets.Infrastructure.Jobs;

using Microsoft.Extensions.Logging;
using Quartz;
using WroclawTheatreTickets.Application.Contracts.Repositories;

/// <summary>
/// Background job that removes shows older than 2 years from the database.
/// Runs weekly on Sunday at 3:00 AM.
/// </summary>
public class CleanupOldShowsJob : IJob
{
    private readonly IShowRepository _showRepository;
    private readonly ILogger<CleanupOldShowsJob> _logger;

    public CleanupOldShowsJob(IShowRepository showRepository, ILogger<CleanupOldShowsJob> logger)
    {
        _showRepository = showRepository;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            _logger.LogInformation("Starting cleanup job to remove shows older than 2 years");

            // Calculate cutoff date: 2 years ago from now
            var cutoffDate = DateTime.UtcNow.AddYears(-2);

            // Delete shows older than 2 years
            var deletedCount = await _showRepository.DeleteOlderThanAsync(cutoffDate);

            _logger.LogInformation(
                "Successfully removed {DeletedCount} shows older than {CutoffDate}",
                deletedCount,
                cutoffDate.ToString("yyyy-MM-dd HH:mm:ss"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while executing cleanup job");
            throw;
        }
    }
}

/// <summary>
/// Configuration helper for the cleanup old shows job.
/// </summary>
public static class CleanupOldShowsJobConfig
{
    /// <summary>
    /// Creates a trigger for the cleanup job that runs weekly on Sunday at 3:00 AM.
    /// </summary>
    public static ITrigger CreateWeeklyTrigger()
    {
        return TriggerBuilder.Create()
            .WithIdentity("CleanupOldShowsTrigger")
            .WithCronSchedule("0 3 ? * SUN") // 3:00 AM every Sunday
            .ForJob("CleanupOldShowsJob")
            .Build();
    }

    /// <summary>
    /// Creates the job detail for the cleanup job.
    /// </summary>
    public static IJobDetail CreateJobDetail()
    {
        return JobBuilder.Create<CleanupOldShowsJob>()
            .WithIdentity("CleanupOldShowsJob")
            .WithDescription("Weekly job to clean up shows older than 2 years")
            .Build();
    }
}
