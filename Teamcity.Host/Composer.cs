namespace Teamcity.Host
{
    using System.Diagnostics.CodeAnalysis;
    using Grpc.Core;
    using Pure.DI;
    using static Pure.DI.Lifetime;

    [ExcludeFromCodeCoverage]
    internal static partial class Composer
    {
        static Composer() => DI.Setup()
            .Default(Singleton)
            .Bind<CompositionRoot>().To<CompositionRoot>()
            .Bind<ISession>().To<Session>()
            .Bind<ChannelBase>().To(ctx => new Channel("localhost", ctx.Resolve<ISession>().Port, ChannelCredentials.Insecure));
    }
}