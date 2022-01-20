// ReSharper disable UnusedMember.Local
// ReSharper disable PartialTypeWithSinglePart
namespace TeamCity.CSharpInteractive.Tests.Integration.Core;

using JetBrains.TeamCity.ServiceMessages.Read;
using Pure.DI;

internal static partial class Composer
{
    private static void Setup() => 
        DI.Setup()
            .Default(Lifetime.Singleton)
            .Bind<IFileSystem>().To<FileSystem>()
            .Bind<IServiceMessageParser>().To<ServiceMessageParser>();
}