namespace Teamcity.Host
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Pure.DI;
    using static Pure.DI.Lifetime;

    [ExcludeFromCodeCoverage]
    internal static partial class Composer
    {
        static Composer() => DI.Setup()
            .Default(Singleton)
            .Bind<HostCompositionRoot>().To<HostCompositionRoot>()
            .Bind<IObserver<TT>>().To<PipeObserver<TT>>()
            .Bind<ISession>().To<Session>();
    }
}