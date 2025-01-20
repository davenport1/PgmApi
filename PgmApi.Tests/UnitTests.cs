using System.Text.Json;
using PgmApi.Models;

namespace PgmApi.Tests;

public class UnitTests
{
    [Fact]
    public void PgAutoFailover_MonitorJson_ParsingTest()
    {
        // Arrange
        PgmStatusCommandResult commandResult = new();
        string commandOutput = @"
        18:08:11 1357597 INFO  pg_autoctl is running with pid 2919966
        18:08:11 1357597 INFO  Postgres is serving PGDATA ""/var/lib/postgresql/15/data"" on port 5432 with pid 2919979
        {
            ""postgres"": {
                ""pgdata"": ""/var/lib/postgresql/15/data"",
                ""pg_ctl"": ""/usr/lib/postgresql/15/bin/pg_ctl"",
                ""version"": ""15.10"",
                ""host"": ""/var/run/postgresql"",
                ""port"": 5432,
                ""proxyport"": 0,
                ""pid"": 2919979,
                ""in_recovery"": false,
                ""control"": {
                    ""version"": 0,
                    ""catalog_version"": 0,
                    ""system_identifier"": ""0""
                },
                ""postmaster"": {
                    ""status"": ""ready""
                }
            },
            ""pg_autoctl"": {
                ""pid"": 2919966,
                ""status"": ""running"",
                ""pgdata"": ""/var/lib/postgresql/15/data"",
                ""version"": ""2.0"",
                ""semId"": 131083,
                ""services"": [
                    {
                        ""name"": ""postgres"",
                        ""pid"": 2919969,
                        ""status"": ""running"",
                        ""version"": ""2.0"",
                        ""pgautofailover"": ""2.0""
                    },
                    {
                        ""name"": ""listener"",
                        ""pid"": 2919970,
                        ""status"": ""running"",
                        ""version"": ""2.0"",
                        ""pgautofailover"": ""2.0""
                    }
                ]
            }
        }";

        // Act
        commandResult.ParseResult(commandOutput);
        
        // Assert
        Assert.NotNull(commandResult.Status);
        Assert.NotNull(commandResult.Status.Postgres);
        Assert.NotNull(commandResult.Status.Postgres.Postmaster);
        Assert.NotNull(commandResult.Status.PgAutoCtl);
        Assert.NotNull(commandResult.Status.PgAutoCtl.Services);
        
        Assert.Equal("ready", commandResult.Status.Postgres.Postmaster.Status);
        Assert.Equal("running", commandResult.Status.PgAutoCtl.Status);
    }
    
    [Fact]
    public void PgAutoFailover_NodeJson_ParsingTest()
    {
        // Arrange
        PgmStateCommandResult commandResult = new();
        string commandOutput = @"[
            {
                ""health"": 1,
                ""node_id"": 1,
                ""group_id"": 0,
                ""nodehost"": ""100.102.219.66"",
                ""nodename"": ""frodo"",
                ""nodeport"": 5432,
                ""nodecluster"": ""default"",
                ""reported_lsn"": ""E/20E70B90"",
                ""reported_tli"": 17,
                ""formation_kind"": ""pgsql"",
                ""candidate_priority"": 50,
                ""replication_quorum"": true,
                ""current_group_state"": ""primary"",
                ""assigned_group_state"": ""primary""
            },
            {
                ""health"": 1,
                ""node_id"": 2,
                ""group_id"": 0,
                ""nodehost"": ""100.67.92.57"",
                ""nodename"": ""samwise"",
                ""nodeport"": 5432,
                ""nodecluster"": ""default"",
                ""reported_lsn"": ""E/20E70B90"",
                ""reported_tli"": 17,
                ""formation_kind"": ""pgsql"",
                ""candidate_priority"": 50,
                ""replication_quorum"": true,
                ""current_group_state"": ""secondary"",
                ""assigned_group_state"": ""secondary""
            }
        ]";

        // Act
        commandResult.ParseResult(commandOutput);

        // Assert
        Assert.NotNull(commandResult.NodeState);
        Assert.NotEmpty(commandResult.NodeState);
        Assert.Equal(2, commandResult.NodeState.Count);
        Assert.Equal(1, commandResult.NodeState[0].Health);
        Assert.Equal(1, commandResult.NodeState[1].Health);
    }

    [Fact]
    public void PgmStatusCommandResult_InvalidJson_SetsError()
    {
        // Arrange
        PgmStatusCommandResult commandResult = new();
        string invalidJson = @"
        18:08:11 1357597 INFO  pg_autoctl is running with pid 2919966
        18:08:11 1357597 INFO  Postgres is serving PGDATA ""/var/lib/postgresql/15/data"" on port 5432 with pid 2919979
        {
            ""invalid"": true
        }";

        // Act
        commandResult.ParseResult(invalidJson);

        // Assert
        Assert.Equal(500, commandResult.ExitCode);
        Assert.Equal("Unable to parse pg_autoctl status", commandResult.Error);
    }

    [Fact]
    public void PgmStatusCommandResult_ErrorHandling_Test()
    {
        // Arrange
        PgmStatusCommandResult commandResult = new();
        string errorMessage = "Command failed";

        // Act
        commandResult.ParseError(errorMessage);
        commandResult.SetExitCode(1);

        // Assert
        Assert.Equal(1, commandResult.ExitCode);
        Assert.Equal(errorMessage, commandResult.Error);
    }

    [Fact]
    public void PgmStateCommandResult_EmptyJson_ReturnsEmptyList()
    {
        // Arrange
        PgmStateCommandResult commandResult = new();
        string emptyJson = "[]";

        // Act
        commandResult.ParseResult(emptyJson);

        // Assert
        Assert.NotNull(commandResult.NodeState);
        Assert.Empty(commandResult.NodeState);
    }

    [Fact]
    public void PgmStateCommandResult_ErrorHandling_Test()
    {
        // Arrange
        PgmStateCommandResult commandResult = new();
        string errorMessage = "Command failed";

        // Act
        commandResult.ParseError(errorMessage);
        commandResult.SetExitCode(1);

        // Assert
        Assert.Equal(1, commandResult.ExitCode);
        Assert.Equal(errorMessage, commandResult.Error);
    }

    [Fact]
    public void MonitorHealthCheck_UnhealthyStatus_Test()
    {
        // Arrange
        var commandResult = new PgmStatusCommandResult();
        commandResult.ParseResult(@"
        18:08:11 1357597 INFO  pg_autoctl is running with pid 2919966
        18:08:11 1357597 INFO  Postgres is serving PGDATA ""/var/lib/postgresql/15/data"" on port 5432 with pid 2919979
        {
            ""postgres"": {
                ""pgdata"": ""/var/lib/postgresql/15/data"",
                ""pg_ctl"": ""/usr/lib/postgresql/15/bin/pg_ctl"",
                ""version"": ""15.10"",
                ""host"": ""/var/run/postgresql"",
                ""port"": 5432,
                ""proxyport"": 0,
                ""pid"": 2919979,
                ""in_recovery"": false,
                ""control"": {
                    ""version"": 0,
                    ""catalog_version"": 0,
                    ""system_identifier"": ""0""
                }
            },
            ""pg_autoctl"": {
                ""pid"": 2919966,
                ""status"": ""stopped"",
                ""pgdata"": ""/var/lib/postgresql/15/data"",
                ""version"": ""2.0"",
                ""semId"": 131083,
                ""services"": []
            }
        }");

        // Assert
        Assert.NotNull(commandResult.Status);
        Assert.NotNull(commandResult.Status.PgAutoCtl);
        Assert.Equal("stopped", commandResult.Status.PgAutoCtl.Status);
        Assert.Null(commandResult.Status.Postgres?.Postmaster);
    }

    [Fact]
    public void NodeHealthCheck_UnhealthyNode_Test()
    {
        // Arrange
        var commandResult = new PgmStateCommandResult();
        commandResult.ParseResult(@"[
            {
                ""health"": 0,
                ""node_id"": 1,
                ""group_id"": 0,
                ""nodehost"": ""100.102.219.66"",
                ""nodename"": ""frodo"",
                ""nodeport"": 5432,
                ""nodecluster"": ""default"",
                ""reported_lsn"": ""E/20E70B90"",
                ""reported_tli"": 17,
                ""formation_kind"": ""pgsql"",
                ""candidate_priority"": 50,
                ""replication_quorum"": true,
                ""current_group_state"": ""primary"",
                ""assigned_group_state"": ""primary""
            }
        ]");

        // Assert
        Assert.NotNull(commandResult.NodeState);
        Assert.Single(commandResult.NodeState);
        Assert.Equal(0, commandResult.NodeState[0].Health);
    }

    [Fact]
    public void PgmStatusCommandResult_NoPostmaster_ReturnsUnhealthy()
    {
        // Arrange
        PgmStatusCommandResult commandResult = new();
        string commandOutput = @"
        {
            ""postgres"": {
                ""pgdata"": ""/var/lib/postgresql/15/data"",
                ""pg_ctl"": ""/usr/lib/postgresql/15/bin/pg_ctl"",
                ""version"": ""15.10"",
                ""host"": ""/var/run/postgresql"",
                ""port"": 5432,
                ""proxyport"": 0,
                ""pid"": 2919979,
                ""in_recovery"": false,
                ""control"": {
                    ""version"": 0,
                    ""catalog_version"": 0,
                    ""system_identifier"": ""0""
                }
            },
            ""pg_autoctl"": {
                ""pid"": 2919966,
                ""status"": ""running"",
                ""pgdata"": ""/var/lib/postgresql/15/data"",
                ""version"": ""2.0"",
                ""semId"": 131083,
                ""services"": []
            }
        }";

        // Act
        commandResult.ParseResult(commandOutput);
        
        // Assert
        Assert.NotNull(commandResult.Status);
        Assert.NotNull(commandResult.Status.Postgres);
        Assert.Null(commandResult.Status.Postgres.Postmaster);
    }

    [Fact]
    public void PgmStatusCommandResult_NoServices_ReturnsEmptyList()
    {
        // Arrange
        PgmStatusCommandResult commandResult = new();
        string commandOutput = @"
        {
            ""postgres"": {
                ""pgdata"": ""/var/lib/postgresql/15/data"",
                ""pg_ctl"": ""/usr/lib/postgresql/15/bin/pg_ctl"",
                ""version"": ""15.10"",
                ""host"": ""/var/run/postgresql"",
                ""port"": 5432,
                ""proxyport"": 0,
                ""pid"": 2919979,
                ""in_recovery"": false,
                ""control"": {
                    ""version"": 0,
                    ""catalog_version"": 0,
                    ""system_identifier"": ""0""
                },
                ""postmaster"": {
                    ""status"": ""ready""
                }
            },
            ""pg_autoctl"": {
                ""pid"": 2919966,
                ""status"": ""running"",
                ""pgdata"": ""/var/lib/postgresql/15/data"",
                ""version"": ""2.0"",
                ""semId"": 131083,
                ""services"": []
            }
        }";

        // Act
        commandResult.ParseResult(commandOutput);
        
        // Assert
        Assert.NotNull(commandResult.Status);
        Assert.NotNull(commandResult.Status.PgAutoCtl);
        Assert.Empty(commandResult.Status.PgAutoCtl.Services);
    }

    [Fact]
    public void PgmNodeState_DefaultValues_Test()
    {
        // Arrange & Act
        var nodeState = new PgmNodeState();

        // Assert
        Assert.Equal(0, nodeState.Health);
        Assert.Equal(0, nodeState.NodeId);
        Assert.Equal(0, nodeState.GroupId);
        Assert.Equal(string.Empty, nodeState.NodeHost);
        Assert.Equal(string.Empty, nodeState.NodeName);
        Assert.Equal(0, nodeState.NodePort);
        Assert.Equal(string.Empty, nodeState.NodeCluster);
        Assert.Equal(string.Empty, nodeState.ReportedLsn);
        Assert.Equal(0, nodeState.ReportedTli);
        Assert.Equal(string.Empty, nodeState.FormationKind);
        Assert.Equal(0, nodeState.CandidatePriority);
        Assert.False(nodeState.ReplicationQuorum);
        Assert.Equal(string.Empty, nodeState.CurrentGroupState);
        Assert.Equal(string.Empty, nodeState.AssignedGroupState);
    }

    [Fact]
    public void PgmStateCommandResult_InvalidJson_HandlesError()
    {
        // Arrange
        PgmStateCommandResult commandResult = new();
        string invalidJson = "This is not valid JSON";

        // Act & Assert
        Assert.Throws<JsonException>(() => commandResult.ParseResult(invalidJson));
    }
}