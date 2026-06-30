using Api.Hubs;
using Api.Jobs;
using Api.Services;
using Application;
using Application.Shared.Interfaces;
using Hangfire;
using Hangfire.SqlServer;
using Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.IdentityModel.Tokens.Jwt;

//JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear(); //dont change name of claims in token to Microsoft specific ones (e.g. "sub" to ClaimTypes.NameIdentifier) and use them as they are in the token.

var builder = WebApplication.CreateBuilder(args);

//Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddLogging(options =>
{
    options.AddConsole();
});

// SignalR
builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, UserIdProvider>();
builder.Services.AddScoped<IRealtimeNotificationService, SignalRNotificationService>();

builder.Services.Configure<ReleaseStageMonitorOptions>(
    builder.Configuration.GetSection(ReleaseStageMonitorOptions.SectionName));

builder.Services.AddHangfire(config => config
    .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
    .UseSimpleAssemblyNameTypeSerializer()
    .UseRecommendedSerializerSettings()
    .UseSqlServerStorage(builder.Configuration.GetConnectionString("Default"), new SqlServerStorageOptions
    {
        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
        QueuePollInterval = TimeSpan.Zero,
        UseRecommendedIsolationLevel = true,
        DisableGlobalLocks = true
    }));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<ReleaseStageDeadlineJob>();

//CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); //for tokens and cookies (required for SignalR)
    });
});

var app = builder.Build();

//Middlewares
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "PD Management API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<NotificationHub>("/hubs/notifications");

// Hangfire dashboard at /hangfire (allows local connections only by default).
app.UseHangfireDashboard("/hangfire");

// Register (or update) the recurring deadline-monitor job from configuration.
var monitorOptions = app.Services.GetRequiredService<IOptions<ReleaseStageMonitorOptions>>().Value;
var recurringJobs = app.Services.GetRequiredService<IRecurringJobManager>();

if (monitorOptions.Enabled)
{
    recurringJobs.AddOrUpdate<ReleaseStageDeadlineJob>(
        ReleaseStageDeadlineJob.RecurringJobId,
        job => job.RunAsync(CancellationToken.None),
        monitorOptions.Cron);
}
else
{
    // Ensure a previously-registered job is removed when disabled.
    recurringJobs.RemoveIfExists(ReleaseStageDeadlineJob.RecurringJobId);
}

app.Run();
