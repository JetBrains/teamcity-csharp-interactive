// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedPositionalProperty.Global
namespace DotNet;

using System.Text;
using JetBrains.TeamCity.ServiceMessages;

[Immutype.Target]
public readonly record struct BuildMessage(
    BuildMessageState State,
    IServiceMessage? ServiceMessage = default,
    string Text = "",
    string ErrorDetails = "")
{
    public override string ToString()
    {
        var sb = new StringBuilder();
        switch (State)
        {
            case BuildMessageState.ServiceMessage:
                sb.Append(State.ToString());
                sb.Append(' ');
                break;

            case BuildMessageState.StdOut:
                sb.Append(Text);
                break;
                
            case BuildMessageState.StdError:
                sb.Append(State.ToString());
                sb.Append(' ');
                sb.Append(Text);
                break;

            case BuildMessageState.Warning:
                sb.Append(State.ToString());
                sb.Append(' ');
                sb.Append(Text);
                break;

            case BuildMessageState.Failure:
            case BuildMessageState.BuildProblem:
                sb.Append(State.ToString());
                sb.Append(' ');
                sb.Append(Text);
                if (!string.IsNullOrWhiteSpace(ErrorDetails))
                {
                    sb.Append(": ");
                    sb.Append(ErrorDetails);
                }

                break;

            default:
                sb.Append(State);
                break;
        }

        return sb.ToString();
    }
}