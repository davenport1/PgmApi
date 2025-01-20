using PgmApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks()
    .AddMonitorHealthChecks("pg_auto_failover monitor")
    .AddNodeHealthChecks("pg_auto_failover nodes");

var app = builder.Build();

app.MapEndpoints();

app.Run();
