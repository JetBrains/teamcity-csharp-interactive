// ReSharper disable UnusedMember.Local
namespace TeamCity.CSharpInteractive.Tests.Integration.Core
{
    using System;
    using System.Diagnostics;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using Pure.DI;
    using static Tags;

    internal static partial class Composer
    {
        [Conditional("DI")]
        private static void Setup() => DI.Setup()
            .Default(Lifetime.Singleton)
            .Bind<string>(WorkingDirectory).To(_ => Environment.CurrentDirectory)
            .Bind<char>(ArgumentsSeparatorChar).To(_ => ' ')
            .Bind<IFileSystem>().To<FileSystem>()
            .Bind<IProcessFactory>().To<ProcessFactory>()
            .Bind<Func<System.Diagnostics.Process, IProcess>>().To(_ => new Func<System.Diagnostics.Process, IProcess>(process => new Process(process)))
            .Bind<IProcessRunner>().To<CsiProcessRunner>()
            .Bind<IServiceMessageParser>().To<ServiceMessageParser>();
    }
}