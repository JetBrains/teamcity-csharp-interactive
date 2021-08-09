// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Host;
    using Grpc.Core;
    using Console = Host.ConsoleService;

    [ExcludeFromCodeCoverage]
    internal class ServicesHost: IActive
    {
        private readonly ILog<ServicesHost> _log;
        private readonly ISession _session;
        private readonly FlowService.FlowServiceBase _flowService;
        private readonly ConsoleService.ConsoleServiceBase _consoleService;
        private readonly Teamcity.Host.LogService.LogServiceBase _logService;

        public ServicesHost(
            ILog<ServicesHost> log,
            ISession session,
            FlowService.FlowServiceBase flowService,
            ConsoleService.ConsoleServiceBase consoleService,
            Teamcity.Host.LogService.LogServiceBase logService)
        {
            _log = log;
            _session = session;
            _flowService = flowService;
            _consoleService = consoleService;
            _logService = logService;
        }

        public IDisposable Activate()
        {
            var server = new Server
            {
                Services =
                {
                    FlowService.BindService(_flowService),
                    ConsoleService.BindService(_consoleService),
                    Teamcity.Host.LogService.BindService(_logService)
                },
                Ports =
                {
                    new ServerPort("localhost",0, ServerCredentials.Insecure)
                }
            };
            
            server.Start();
            var boundPort = server.Ports.Single().BoundPort;
            _session.Port = boundPort;
            _log.Trace($"Starting gRPC on port \"{boundPort}\".");

            return Disposable.Create(() =>
            {
                _log.Trace($"Stopping gRPC on port \"{boundPort}\".");
                server.ShutdownAsync().Wait();
                _log.Trace($"Stopped gRPC on port \"{boundPort}\".");
            });
        }
    }
}