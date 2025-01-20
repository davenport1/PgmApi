using System.Text.Json.Serialization;

namespace PgmApi.Models;

/// <summary>
/// Represents the state of a PostgreSQL node in a managed cluster.
/// </summary>
public class PgmNodeState
{
    /// <summary>
    /// Gets or sets the health status of the node (0-100).
    /// </summary>
    [JsonPropertyName("health")] public int Health { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the node.
    /// </summary>
    [JsonPropertyName("node_id")] public int NodeId { get; set; }

    /// <summary>
    /// Gets or sets the group identifier this node belongs to.
    /// </summary>
    [JsonPropertyName("group_id")] public int GroupId { get; set; }

    /// <summary>
    /// Gets or sets the hostname where the node is running.
    /// </summary>
    [JsonPropertyName("nodehost")] public string NodeHost { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the friendly name of the node.
    /// </summary>
    [JsonPropertyName("nodename")] public string NodeName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the port number the node is listening on.
    /// </summary>
    [JsonPropertyName("nodeport")] public int NodePort { get; set; }

    /// <summary>
    /// Gets or sets the cluster name this node belongs to.
    /// </summary>
    [JsonPropertyName("nodecluster")] public string NodeCluster { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Log Sequence Number (LSN) reported by this node.
    /// </summary>
    [JsonPropertyName("reported_lsn")] public string ReportedLsn { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Timeline ID (TLI) reported by this node.
    /// </summary>
    [JsonPropertyName("reported_tli")] public int ReportedTli { get; set; }

    /// <summary>
    /// Gets or sets the formation kind of the node (e.g., primary, secondary).
    /// </summary>
    [JsonPropertyName("formation_kind")] public string FormationKind { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the priority of this node when considering it as a candidate for promotion.
    /// </summary>
    [JsonPropertyName("candidate_priority")] public int CandidatePriority { get; set; }

    /// <summary>
    /// Gets or sets whether this node participates in replication quorum decisions.
    /// </summary>
    [JsonPropertyName("replication_quorum")] public bool ReplicationQuorum { get; set; }

    /// <summary>
    /// Gets or sets the current state of the group this node belongs to.
    /// </summary>
    [JsonPropertyName("current_group_state")] public string CurrentGroupState { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the assigned state of the group this node belongs to.
    /// </summary>
    [JsonPropertyName("assigned_group_state")] public string AssignedGroupState { get; set; } = string.Empty;
}