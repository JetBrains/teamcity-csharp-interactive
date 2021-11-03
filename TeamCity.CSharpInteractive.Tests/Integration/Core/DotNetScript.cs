namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using Contracts;

    internal record DotNetScript: CommandLine
    {
        public static readonly CommandLine Shared = new DotNetScript();
        
        private DotNetScript()
            : base("dotnet", "dotnet-csi.dll")
        {
        }
    }
}