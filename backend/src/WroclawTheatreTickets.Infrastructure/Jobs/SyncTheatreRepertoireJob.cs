using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;
using System.Net.Http.Json;
using WroclawTheatreTickets.Application.Contracts.Dtos;
using WroclawTheatreTickets.Application.Contracts.Services;
using WroclawTheatreTickets.Application.UseCases.Shows.Commands;

namespace WroclawTheatreTickets.Infrastructure.Jobs;

/// <summary>
/// Daily background job that synchronizes theatre repertoire data
/// Uses ITheatreRepertoireSyncService to handle API communication and data persistence
/// 
/// Runs daily at 2:00 AM and orchestrates the synchronization process
/// </summary>
public class SyncTheatreRepertoireJob : IJob
{
    private readonly ITheatreRepertoireSyncService _syncService;
    private readonly ILogger<SyncTheatreRepertoireJob> _logger;

    public SyncTheatreRepertoireJob(
        ITheatreRepertoireSyncService syncService,
        ILogger<SyncTheatreRepertoireJob> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        _logger.LogInformation("Starting theatre repertoire synchronization job");

        try
        {
            var result = await _syncService.SyncTheatreRepertoireAsync();

            if (result.IsSuccess)
            {
                _logger.LogInformation(
                    "Theatre repertoire synchronization completed successfully. " +
                    "Success: {SuccessCount}, Failed: {FailureCount}, Total: {TotalEventsProcessed}",
                    result.SuccessCount, result.FailureCount, result.TotalEventsProcessed);
            }
            else
            {
                _logger.LogError("Theatre repertoire synchronization failed: {ErrorMessage}", result.ErrorMessage);
                throw new InvalidOperationException(result.ErrorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Theatre repertoire synchronization job failed");
            throw;
        }
    }
}

/// <summary>
/// Job configuration for Quartz.NET scheduler
/// Defines the trigger (daily at 2:00 AM) and job properties
/// </summary>
public static class TheatreRepertoireJobConfig
{
    public static JobKey JobKey => new(nameof(SyncTheatreRepertoireJob));
    public static TriggerKey TriggerKey => new($"{nameof(SyncTheatreRepertoireJob)}-Trigger");

    /// <summary>
    /// Create a daily trigger that runs at 2:00 AM
    /// Cron: 0 2 * * ? (at 02:00:00 every day)
    /// </summary>
    public static ITrigger CreateDailyTrigger()
    {
        return TriggerBuilder.Create()
            .WithIdentity(TriggerKey)
            .WithCronSchedule("0 2 * * ?") // Daily at 2:00 AM
            .WithDescription("Daily theatre repertoire synchronization trigger")
            .StartNow()
            .Build();
    }

    /// <summary>
    /// Create the job detail configuration
    /// </summary>
    public static IJobDetail CreateJobDetail()
    {
        return JobBuilder.Create<SyncTheatreRepertoireJob>()
            .WithIdentity(JobKey)
            .WithDescription("Synchronizes theatre repertoire from API to local database")
            .StoreDurably(true)
            .Build();
    }
}
