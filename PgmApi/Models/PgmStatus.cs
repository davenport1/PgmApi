using System.Text.Json.Serialization;

// This is a collection of classes for json deserialization of the pg_autoctl status --json command
// to be used with the BashSharp library for parsing the output and give to the healthchecks ui

namespace PgmApi.Models;

public class PgAutoCtlStatus
{
    [JsonPropertyName("postgres")] public PostgresInfo? Postgres { get; set; } 

    [JsonPropertyName("pg_autoctl")] public PgAutoCtlInfo? PgAutoCtl { get; set; }
}

public class PostgresInfo
{
    [JsonPropertyName("pgdata")] public string PgData { get; set; } = string.Empty;

    [JsonPropertyName("pg_ctl")] public string PgCtl { get; set; } = string.Empty;

    [JsonPropertyName("version")] public string Version { get; set; } = string.Empty;

    [JsonPropertyName("host")] public string Host { get; set; } = string.Empty;

    [JsonPropertyName("port")] public int Port { get; set; }

    [JsonPropertyName("proxyport")] public int ProxyPort { get; set; }

    [JsonPropertyName("pid")] public int Pid { get; set; }

    [JsonPropertyName("in_recovery")] public bool InRecovery { get; set; }

    [JsonPropertyName("control")] public ControlInfo? Control { get; set; }

    [JsonPropertyName("postmaster")] public PostmasterInfo? Postmaster { get; set; }
}

public class ControlInfo
{
    [JsonPropertyName("version")] public int Version { get; set; }

    [JsonPropertyName("catalog_version")] public int CatalogVersion { get; set; }

    [JsonPropertyName("system_identifier")] public string SystemIdentifier { get; set; } = string.Empty;
}

public class PostmasterInfo
{
    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;
}

public class PgAutoCtlInfo
{
    [JsonPropertyName("pid")] public int Pid { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

    [JsonPropertyName("pgdata")] public string PgData { get; set; } = string.Empty;

    [JsonPropertyName("version")] public string Version { get; set; } = string.Empty;

    [JsonPropertyName("semId")] public int SemId { get; set; }

    [JsonPropertyName("services")] public List<ServiceInfo> Services { get; set; } = [];
}

public class ServiceInfo
{
    [JsonPropertyName("name")] public string Name { get; set; } = string.Empty;

    [JsonPropertyName("pid")] public int Pid { get; set; }

    [JsonPropertyName("status")] public string Status { get; set; } = string.Empty;

    [JsonPropertyName("version")] public string Version { get; set; } = string.Empty;

    [JsonPropertyName("pgautofailover")] public string PgAutoFailover { get; set; } = string.Empty;
}
