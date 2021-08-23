namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    internal readonly struct ExitCode
    {
        public readonly int Value;

        public ExitCode(int value) => Value = value;

        public static implicit operator ExitCode(int exitCode) => new ExitCode(exitCode);

        public override string ToString() => Value.ToString();
    }
}
