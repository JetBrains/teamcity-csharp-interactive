// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using Host;
    using Grpc.Core;
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
            var server = new Server
            {
                Services =
                {
                    Flow.BindService(_flowService),
                    Console.BindService(_consoleService),
                    Teamcity.Host.Log.BindService(_logService)
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