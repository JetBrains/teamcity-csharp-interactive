// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable InconsistentNaming
// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
namespace Dotnet
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public record BuildResult(
        int? ExitCode,
        IEnumerable<BuildMessage> Messages,
        IEnumerable<TestResult> Tests)
    {
        public bool Success => 
            ExitCode == 0
            && Messages.All(i => i.State <= BuildMessageState.Warning)
            && Tests.All(i => i.State != TestState.Failed);

        public override string ToString()
        {
            var sb = new StringBuilder(Success ? "Build succeeded" : "Build failed");
            foreach (var reason in FormatReasons(GetReasons()))
            {
                sb.Append(reason);
            }

            sb.Append('.');
            return sb.ToString();
        }

        public static implicit operator string(BuildResult it) => it.ToString();

        private IEnumerable<string> GetReasons()
        {
            var errors = Messages.Count(i => i.State == BuildMessageState.Error);
            if (errors > 0)
            {
                yield return $"{errors} errors";
            }

            var warnings = Messages.Count(i => i.State == BuildMessageState.Warning);
            if (warnings > 0)
            {
                yield return $"{warnings} warnings";
            }

            var failedTests = Tests.Count(i => i.State == TestState.Failed);
            if (failedTests > 0)
            {
                yield return $"{failedTests} failed tests";
            }
            
            var IgnoredTests = Tests.Count(i => i.State == TestState.Ignored);
            if (IgnoredTests > 0)
            {
                yield return $"{IgnoredTests} ignored tests";
            }

            if (ExitCode != 0)
            {
                yield return $"exit code {ExitCode}";
            }
        }

        private static IEnumerable<string> FormatReasons(IEnumerable<string> reasons)
        {
            var counter = 0;
            foreach (var reason in reasons)
            {
                switch (counter)
                {
                    case 0:
                        yield return " with ";
                        yield return reason;
                        break;
                        
                    default:
                        yield return " and with ";
                        yield return reason;
                        break;
                }

                counter++;
            }
        }
    }
}