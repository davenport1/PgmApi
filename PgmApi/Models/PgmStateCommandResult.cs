using System.Text.Json;
using BashSharp.Interfaces;

namespace PgmApi.Models;

/// <inheritdoc />
public class PgmStateCommandResult : ICommandResult
{
    /// <summary>
    /// Exit code if reported
    /// </summary>
    public int ExitCode { get; private set; }

    /// <summary>
    /// List of node's states reported by the show state command
    /// </summary>
    public List<PgmNodeState> NodeState { get; private set; } = [];

    /// <summary>
    /// Error message if reported by CLI
    /// </summary>
    public string Error { get; private set; } = string.Empty;

    /// <inheritdoc />
    public void SetExitCode(int exitCode)
    {
        ExitCode = exitCode;
    }

    /// <summary>
    /// Our result will be in json so we can use the System.Text.Json library to handle our result parsing
    /// </summary>
    /// <param name="result"></param>
    public void ParseResult(string result)
    {
        NodeState = JsonSerializer.Deserialize<List<PgmNodeState>>(result) ?? [];
    }

    /// <inheritdoc />
    public void ParseError(string error)
    {
        Error = error;
    }
}