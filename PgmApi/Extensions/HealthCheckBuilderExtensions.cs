using Microsoft.Extensions.Diagnostics.HealthChecks;
using PgmApi.HealthChecks;

namespace PgmApi.Extensions;

public static class HealthCheckBuilderExtensions
{
    public static IHealthChecksBuilder AddMonitorHealthChecks(
        this IHealthChecksBuilder builder,
        string name,
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null)
    {
        builder.AddCheck<MonitorHealthCheck>(name, failureStatus ?? HealthStatus.Degraded,
            tags ?? Enumerable.Empty<string>());

        return builder;
    }

    public static IHealthChecksBuilder AddNodeHealthChecks(
        this IHealthChecksBuilder builder,
        string name,
        HealthStatus? failureStatus = null,
        IEnumerable<string>? tags = null)
    {
        builder.AddCheck<NodeHealthCheck>(name, failureStatus ?? HealthStatus.Degraded,
            tags ?? Enumerable.Empty<string>());

        return builder;
    }
}