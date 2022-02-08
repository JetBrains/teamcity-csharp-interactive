// ReSharper disable UnusedMember.Local
// ReSharper disable PartialTypeWithSinglePart
namespace TeamCity.CSharpInteractive.Tests.Integration.Core;

using JetBrains.TeamCity.ServiceMessages.Read;

internal static partial class TestComposer
{
    private static void Setup() =>
        Pure.DI.DI.Setup()
            .Default(Pure.DI.Lifetime.Singleton)
            .Bind<IFileSystem>().To<FileSystem>()
            .Bind<IServiceMessageParser>().To<ServiceMessageParser>();
}