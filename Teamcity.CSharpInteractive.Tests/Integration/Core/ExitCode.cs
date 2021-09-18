namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    internal readonly struct ExitCode
    {
        public readonly int Value;

        // ReSharper disable once MemberCanBePrivate.Global
        public ExitCode(int value) => Value = value;

        public static implicit operator ExitCode(int exitCode) => new(exitCode);

        public override string ToString() => Value.ToString();
    }
}
