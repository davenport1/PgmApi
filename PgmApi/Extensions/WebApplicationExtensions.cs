using PgmApi.Models;
using BashSharp;

namespace PgmApi.Extensions;

public static class WebApplicationExtensions
{
    private const string ApiV1 = "/api/v1";
    private const string PgmState = "/pgm/state";
    private const string PgmNodeHealth = PgmState + "/health";
    private const string HealthCheck = "/health";
    private const string PgmStateCommand = "pg_autoctl show state --json";

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
    }
}