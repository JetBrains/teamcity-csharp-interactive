namespace Teamcity.CSharpInteractive
{
    internal class SetVerbosityLevelCommand: ICommand
    {
        public readonly VerbosityLevel VerbosityLevel;

        public SetVerbosityLevelCommand(VerbosityLevel verbosityLevel) => VerbosityLevel = verbosityLevel;
        
        public string Name => $"Set verbosity level to {VerbosityLevel}";

        public CommandKind Kind => CommandKind.SetVerbosityLevel;

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