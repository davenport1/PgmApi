using BashSharp;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PgmApi.Models;

namespace PgmApi.HealthChecks;

public class MonitorHealthCheck : IHealthCheck
{
    private const string PgmStatusCommand = "pg_autoctl status --json";
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        var result = await BashCommandService.ExecuteCommandWithResults<PgmStatusCommandResult>(PgmStatusCommand, cancellationToken: cancellationToken);
        Dictionary<string, object> data = new();
        data.Add("pg_autoctl status", result.Status);

        bool healthy = result.Status.Postgres is not null 
                       && result.Status.PgAutoCtl is not null 
                       && result.Status.Postgres.Postmaster is not null
                       && result.Status.Postgres.Postmaster.Status == "ready"
                       && result.Status.PgAutoCtl.Status == "running";
        
        HealthStatus healthStatus = healthy ? HealthStatus.Healthy : HealthStatus.Unhealthy;
        
        return new HealthCheckResult(
            healthStatus,
            description: "PgmApi Monitor Health Check",
            exception: null,
            data: data);
    }
}