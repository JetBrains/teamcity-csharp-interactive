// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cmd;
using Contracts;

internal class ProcessMonitor : IProcessMonitor
{
    private readonly ILog<ProcessMonitor> _log;
    private Text _info = Text.Empty;
    private Text[] _header = Array.Empty<Text>();
    private int _processId;

    public ProcessMonitor(ILog<ProcessMonitor> log) => _log = log;

    public void Starting(IStartInfo startInfo, int processId)
    {
        _processId = processId;
        var sb = new StringBuilder();
        if (!string.IsNullOrWhiteSpace(startInfo.WorkingDirectory))
        {
            sb.Append(Escape(startInfo.WorkingDirectory));
            sb.Append(" => ");
        }

        sb.Append($"process {processId} [");
        sb.Append(Escape(startInfo.ExecutablePath));
        foreach (var arg in startInfo.Args)
        {
            sb.Append(' ');
            sb.Append(Escape(arg));
        }
        sb.Append(']');

        // ReSharper disable once InvertIf
        if (startInfo.Vars.Any())
        {
            sb.Append(" with environment variables ");
            foreach (var (name, value) in startInfo.Vars)
            {
                sb.Append('[');
                sb.Append(name);
                sb.Append('=');
                sb.Append(value);
                sb.Append(']');
            }
        }

        _info = new Text(sb.ToString());
        _header = GetHeader(startInfo).ToArray();
    }

    public void Started() => _log.Info(_header);

    public void Finished(long elapsedMilliseconds, ProcessState state, int? exitCode = default)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch (state)
        {
            case ProcessState.Success:
                _log.Info(GetFooter(exitCode ?? 0, elapsedMilliseconds, state).ToArray().WithDefaultColor(Color.Success));
                break;

            case ProcessState.Fail:
                // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
                if (exitCode.HasValue)
                {
                    _log.Error(ErrorId.Process, GetFooter(exitCode.Value, elapsedMilliseconds, state).ToArray());
                }
                else
                {
                    _log.Error(ErrorId.Process, new[] {_info, new Text(" - failed to start.")}.ToArray());
                }

                break;
            
            case ProcessState.Cancel:
                _log.Warning(_info, new Text(" - canceled."));
                break;

            default:
                _log.Info(GetFooter(exitCode ?? 0, elapsedMilliseconds, state).ToArray().WithDefaultColor(Color.Highlighted));
                break;
        }
    }
    
    private IEnumerable<Text> GetHeader(IStartInfo startInfo)
    {
        yield return new Text($"Starting process {_processId}: ", Color.Header);
        yield return new Text(Escape(startInfo.ExecutablePath), Color.Header);
        foreach (var arg in startInfo.Args)
        {
            yield return Text.Space;
            yield return new Text(Escape(arg));
        }
            
        yield return Text.NewLine;
        yield return new Text("in directory: ");
        yield return new Text(Escape(startInfo.WorkingDirectory));
    }

    private IEnumerable<Text> GetFooter(int exitCode, long elapsedMilliseconds, ProcessState? state)
    {
        var stateText = state switch
        {
            ProcessState.Success => "finished successfully",
            ProcessState.Fail => "failed",
            _ => "finished"
        };

        yield return new Text($"Process {_processId} ");
        yield return new Text(stateText);
        yield return new Text($" (in {elapsedMilliseconds} ms) with exit code {exitCode}.");
    }
    
    private static string Escape(string text) => !text.TrimStart().StartsWith("\"") && text.Contains(' ') ? $"\"{text}\"" : text;
}