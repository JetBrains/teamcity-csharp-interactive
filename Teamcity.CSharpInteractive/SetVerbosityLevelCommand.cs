namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class SetVerbosityLevelCommand: ICommand
    {
        public readonly VerbosityLevel VerbosityLevel;

        public SetVerbosityLevelCommand(VerbosityLevel verbosityLevel) => VerbosityLevel = verbosityLevel;
        
        public string Name => $"Set verbosity level to {VerbosityLevel}";
        
        public bool Internal => false;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            SetVerbosityLevelCommand other = (SetVerbosityLevelCommand) obj;
            return VerbosityLevel == other.VerbosityLevel;
        }

        public override int GetHashCode() => (int) VerbosityLevel;
        
        public override string ToString() => Name;
    }
}