// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedPositionalProperty.Global
namespace Dotnet
{
    using System.Text;
    using JetBrains.TeamCity.ServiceMessages;

    [Immutype.Target]
    public readonly record struct BuildMessage(
        BuildMessageState State,
        IServiceMessage? ServiceMessage = default,
        string Text = "",
        string ErrorDetails = "")
    {
        public static implicit operator string(BuildMessage it) => it.ToString();
        
        public override string ToString()
        {
            var sb = new StringBuilder();
            switch (State)
            {
                case BuildMessageState.ServiceMessage:
                    sb.Append("Service Message");
                    break;

                case BuildMessageState.StdOut:
                    sb.Append(Text);
                    break;
                
                case BuildMessageState.StdErr:
                    sb.Append("StdErr ");
                    sb.Append(Text);
                    break;

                case BuildMessageState.Warning:
                    sb.Append("Warning ");
                    sb.Append(Text);
                    break;

                case BuildMessageState.Failure:
                case BuildMessageState.BuildProblem:
                case BuildMessageState.Error:
                    sb.Append("Error ");
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
}