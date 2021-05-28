namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class ScriptCommand: ICommand
    {
        private readonly string _originName;
        public readonly string Script;

        public ScriptCommand(string originName, string script)
        {
            _originName = originName;
            Script = script;
        }

        public string Name => _originName;

        public CommandKind Kind => CommandKind.Script;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            ScriptCommand other = (ScriptCommand) obj;
            return _originName == other._originName && Script == other.Script;
        }

        public override int GetHashCode() => HashCode.Combine(_originName, Script);
    }
}