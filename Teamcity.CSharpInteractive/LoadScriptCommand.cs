namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class LoadScriptCommand: ICommand
    {
        public readonly string ScriptFileName;

        public LoadScriptCommand(string scriptFileName) => ScriptFileName = scriptFileName;
        
        public string Name => $"Load script from {ScriptFileName}";

        public CommandKind Kind => CommandKind.LoadScript;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            LoadScriptCommand other = (LoadScriptCommand) obj;
            return ScriptFileName == other.ScriptFileName;
        }

        public override int GetHashCode() => ScriptFileName.GetHashCode();
    }
}