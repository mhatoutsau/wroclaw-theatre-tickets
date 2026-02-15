using WroclawTheatreTickets.Web;
using Serilog;
using WroclawTheatreTickets.Infrastructure.Data;
using WroclawTheatreTickets.Infrastructure.Jobs;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/app.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services
builder.Services.AddApplicationServices(builder.Configuration);

// Configure Quartz.NET for background jobs
builder.Services.AddQuartz(q =>
{
    // Register the theatre repertoire sync job (daily at 2:00 AM)
    var syncJobKey = TheatreRepertoireJobConfig.JobKey;
    q.AddJob<SyncTheatreRepertoireJob>(opts => opts.WithIdentity(syncJobKey));
    q.AddTrigger(opts => opts
        .ForJob(syncJobKey)
        .WithIdentity(TheatreRepertoireJobConfig.TriggerKey)
        .WithCronSchedule("0 0 2 ? * *") // Daily at 2:00 AM
        .WithDescription("Daily theatre repertoire synchronization trigger"));

    // Register the cleanup old shows job (weekly on Sunday at 3:00 AM)
    var cleanupJobKey = new JobKey("CleanupOldShowsJob");
    q.AddJob<CleanupOldShowsJob>(opts => opts.WithIdentity(cleanupJobKey));
    q.AddTrigger(opts => opts
        .ForJob(cleanupJobKey)
        .WithIdentity("CleanupOldShowsTrigger")
        .WithCronSchedule("0 0 3 ? * SUN") // Weekly on Sunday at 3:00 AM
        .WithDescription("Weekly cleanup of shows older than 2 years"));
});

// Add Quartz.NET hosted service
builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);

// Add HttpClientFactory for API calls
builder.Services.AddHttpClient();

var app = builder.Build();

// Initialize database (creates tables if they don't exist)
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TheatreDbContext>();
    dbContext.Database.EnsureCreated();
}

// Check for command-line arguments to force synchronization on startup
if (args.Contains("--force-sync"))
{
    Log.Information("Force synchronization argument detected. Running synchronization on startup...");
    try
    {
        using var scope = app.Services.CreateScope();
        var scheduler = scope.ServiceProvider.GetRequiredService<ISchedulerFactory>().GetScheduler().Result;
        var jobKey = new JobKey("SyncTheatreRepertoireJob");
        await scheduler.TriggerJob(jobKey);
        Log.Information("Synchronization job triggered successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error triggering synchronization job on startup");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// Register API endpoints
Endpoints.RegisterEndpoints(app);

app.Run();

// Make Program accessible to tests
public partial class Program { }

