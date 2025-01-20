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
}