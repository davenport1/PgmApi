using System.Text.Json;
using System.Text.RegularExpressions;
using BashSharp.Interfaces;

namespace PgmApi.Models;

public class PgmStatusCommandResult : ICommandResult
{
    /// <summary>
    /// Exit code if reported
    /// </summary>
    public int ExitCode { get; private set; } = 0;
    
    /// <summary>
    /// Error message if reported by CLI
    /// </summary>
    public string Error { get; private set; } = string.Empty;

    public PgAutoCtlStatus Status { get; set; } = new();
    
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
        string json = Regex.Match(result, @"\{.*\}", RegexOptions.Singleline).Value;
        
        Status = JsonSerializer.Deserialize<PgAutoCtlStatus>(json) ?? new()
        {
            PgAutoCtl = new() { Status = "Unknown" },
        };

        if (Status.PgAutoCtl is null || Status.PgAutoCtl.Status == "Unknown")
        {
            Error = "Unable to parse pg_autoctl status";
            SetExitCode(500);
        }
    }

    /// <inheritdoc />
    public void ParseError(string error)
    {
        Error = error;
    }
}