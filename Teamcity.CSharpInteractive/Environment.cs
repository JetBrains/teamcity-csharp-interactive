// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class Environment : IEnvironment
    {
        public IEnumerable<string> GetCommandLineArgs() => System.Environment.GetCommandLineArgs();
    }
}