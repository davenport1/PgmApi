using PgmApi.Models;
using BashSharp;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

namespace PgmApi.Extensions;

/// <summary>
/// An extension for our web application to take our endpoint mapping out of Program.cs
/// </summary>
public static class WebApplicationExtensions
{
    private const string ApiV1 = "/api/v1";
    private const string PgmState = "/pgm/state";
    private const string PgmNodeHealth = PgmState + "/health";
    private const string HealthCheck = "/health";
    private const string PgmStateCommand = "pg_autoctl show state --json";

    /// <summary>
    /// Map endpoints of the api
    /// </summary>
    /// <param name="app"></param>
    public static void MapEndpoints(this WebApplication app)
    {
        app.MapGet("/", () => "Hello World!");

        app.MapGet(ApiV1 + PgmState, async () => {
            var result = await BashCommandService.ExecuteCommandWithResults<PgmStateCommandResult>(PgmStateCommand);
            return result.NodeState;
        });

        app.MapGet(ApiV1 + PgmNodeHealth, async () => {
            var result = await BashCommandService.ExecuteCommandWithResults<PgmStateCommandResult>(PgmStateCommand);
            foreach (var node in result.NodeState)
            {
                if (node.Health == 0)
                {
                    return false;
                }
            }

            return true;
        });
        
        app.MapGet(ApiV1 + HealthCheck, () => "Hello World!");

        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
        });
    }
}