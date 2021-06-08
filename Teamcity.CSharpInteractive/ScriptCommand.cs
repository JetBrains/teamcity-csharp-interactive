namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class ScriptCommand: ICommand
    {
        public readonly string Script;

        public ScriptCommand(string originName, string script)
        {
            Name = originName;
            Script = script;
        }

        public string Name { get; }

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            ScriptCommand other = (ScriptCommand) obj;
            return Name == other.Name && Script == other.Script;
        }

        public override int GetHashCode() => HashCode.Combine(Name, Script);
        
        public override string ToString() => Name;
    }
}