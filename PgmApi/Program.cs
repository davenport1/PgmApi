using PgmApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

app.MapEndpoints();

app.Run();
