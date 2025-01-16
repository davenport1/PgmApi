# PgmApi

PgmApi is a REST API wrapper for pg_auto_failover's `pg_autoctl` command-line tool. It provides HTTP endpoints to monitor PostgreSQL cluster state and health.

## Prerequisites

- .NET 8.0 SDK
- pg_auto_failover installed and configured
- `pg_autoctl` available in system PATH

## API Endpoints

### Get Cluster State
```http
GET /api/v1/pgm/state

returns detailed state of each node in the cluster
```

### Check Node Health
```http
GET /api/v1/pgm/health

returns true if all nodes are healthy, false otherwise
```

### Health Check
```http
GET /health

returns true if the API is healthy, false otherwise
```

## Running the API

To run the API, use the following command:
```bash
dotnet run --project PgmApi/PgmApi.csproj
```

## Building the API

To build the API, use the following command:
```bash
dotnet build PgmApi/PgmApi.csproj
```

## Running the Tests

To run the tests, use the following command:
```bash
dotnet test PgmApi/PgmApi.csproj
```

## Project Structure

- `PgmApi/` - Main API project
  - `Models/` - Data models for PostgreSQL node state
  - `Extensions/` - Application extension methods
- `PgmApi.Tests/` - Test project

## Dependencies

- BashSharp (1.0.0) - For executing shell commands
- ASP.NET Core 8.0 - Web API framework

## License

MIT