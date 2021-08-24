namespace Teamcity.CSharpInteractive.Tests.Integration.Core
{
    using System;
    using JetBrains.TeamCity.ServiceMessages.Read;
    using Pure.DI;
    using static Tags;

    internal static partial class Composer
    {
        static Composer() => DI.Setup()
            .Default(Lifetime.Singleton)
            .Bind<string>().Tag(WorkingDirectory).To(_ => Environment.CurrentDirectory)
            .Bind<char>().Tag(ArgumentsSeparatorChar).To(_ => ' ')
            .Bind<IFileSystem>().To<FileSystem>()
            .Bind<IProcessFactory>().To<ProcessFactory>()
            .Bind<Func<System.Diagnostics.Process, IProcess>>().To(ctx => new Func<System.Diagnostics.Process, IProcess>(process => new Process(process)))
            .Bind<IProcessRunner>().To<CsiProcessRunner>()
            .Bind<IServiceMessageParser>().To<ServiceMessageParser>();
    }
}