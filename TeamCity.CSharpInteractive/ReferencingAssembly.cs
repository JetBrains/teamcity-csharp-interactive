namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class ReferencingAssembly
    {
        public readonly string Name;
        public readonly string FilePath;

        public ReferencingAssembly(string name, string filePath)
        {
            Name = name;
            FilePath = filePath;
        }

        public override string ToString() => FilePath;
    }
}