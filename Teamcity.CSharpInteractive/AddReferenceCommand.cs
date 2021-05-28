namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class AddReferenceCommand: ICommand
    {
        public readonly string Reference;

        public AddReferenceCommand(string reference) => Reference = reference;

        public string Name => $"Add reference \"{Reference}\"";

        public CommandKind Kind => CommandKind.AddReference;

        public override bool Equals(object? obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            AddReferenceCommand other = (AddReferenceCommand) obj;
            return Reference == other.Reference;
        }

        public override int GetHashCode() => Reference.GetHashCode();
    }
}