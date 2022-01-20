namespace TeamCity.CSharpInteractive;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal class CodeCommand: ICommand
{
    public CodeCommand(bool isInternal = false) => Internal = isInternal;

    public string Name => "Code";
        
    public bool Internal { get; }

    public override string ToString() => Name;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        var other = (CodeCommand) obj;
        return Internal == other.Internal;
    }

    public override int GetHashCode() => Internal.GetHashCode();
}