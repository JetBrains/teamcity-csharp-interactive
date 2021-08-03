// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using GrpcDotNetNamedPipes;
    using Host;
    using Console = Host.Console;

    [ExcludeFromCodeCoverage]
    internal class ServicesHost: IActive
    {
        private readonly ILog<ServicesHost> _log;
        private readonly ISession _session;
        private readonly Flow.FlowBase _flowService;
        private readonly Console.ConsoleBase _consoleService;
        private readonly Teamcity.Host.Log.LogBase _logService;

        public ServicesHost(
            ILog<ServicesHost> log,
            ISession session,
            Flow.FlowBase flowService,
            Console.ConsoleBase consoleService,
            Teamcity.Host.Log.LogBase logService)
        {
            _log = log;
            _session = session;
            _flowService = flowService;
            _consoleService = consoleService;
            _logService = logService;
        }

        public IDisposable Activate()
        {
            var server = new NamedPipeServer(_session.Id);
            
            _log.Trace($"gRPC \"{Flow.Descriptor.FullName}\"");
            Flow.BindService(server.ServiceBinder, _flowService);
            
            _log.Trace($"gRPC \"{Console.Descriptor.FullName}\"");
            Console.BindService(server.ServiceBinder, _consoleService);
            
            _log.Trace($"gRPC \"{Teamcity.Host.Log.Descriptor.FullName}\"");
            Teamcity.Host.Log.BindService(server.ServiceBinder, _logService);
            
            _log.Trace($"Starting gRPC on pipe \"{_session.Id}\".");
            server.Start();
            return Disposable.Create(() =>
            {
                server.Dispose();
                _log.Trace($"Stopped gRPC on pipe \"{_session.Id}\".");
            });
        }
    }
}