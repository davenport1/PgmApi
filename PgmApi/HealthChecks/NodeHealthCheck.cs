using BashSharp;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using PgmApi.Models;

namespace PgmApi.HealthChecks;

public class NodeHealthCheck : IHealthCheck
{
    private const string PgmStateCommand = "pg_autoctl show state --json";
    
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
    {
        var result = await BashCommandService.ExecuteCommandWithResults<PgmStateCommandResult>(PgmStateCommand, cancellationToken: cancellationToken);
        bool nodesUnhealthy = false;
        
        Dictionary<string, object> data = new();
        foreach (var node in result.NodeState)
        {
            data.Add(node.NodeName, node.Health);
            if (node.Health == 0)
            {
                nodesUnhealthy = true;
            }
        }
        
        HealthStatus healthStatus = nodesUnhealthy ? HealthStatus.Unhealthy : HealthStatus.Healthy;

        return new HealthCheckResult(
            healthStatus,
            description: "PgmApi Node Health Check",
            exception: null,
            data: data);
    }
}