using System.Text.Json;
using BashSharp;
using BashSharp.Interfaces;

namespace PgmApi.Models;

public class PgmStateCommandResult : ICommandResult
{
    public int ExitCode { get; private set; }

    public List<PgmNodeState> NodeState { get; private set; } = [];

    public string Error { get; private set; } = string.Empty;

    public void SetExitCode(int exitCode)
    {
        ExitCode = exitCode;
    }

    public void ParseResult(string result)
    {
        NodeState = JsonSerializer.Deserialize<List<PgmNodeState>>(result) ?? [];
    }

    public void ParseError(string error)
    {
        Error = error;
    }
}