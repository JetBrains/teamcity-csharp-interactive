// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Host;

    [ExcludeFromCodeCoverage]
    internal class LogServiceImpl: Teamcity.Host.LogService.LogServiceBase
    {
        private readonly ILog<LogServiceImpl> _log;

        public LogServiceImpl(ILog<LogServiceImpl> log) => _log = log;

        public override Task<Empty> Error(ErrorRequest request, ServerCallContext context)
        {
            _log.Error(new ErrorId(request.ErrorId), new Text(request.Error));
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Warning(WarningRequest request, ServerCallContext context)
        {
            _log.Warning(request.Warning);
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Info(InfoRequest request, ServerCallContext context)
        {
            _log.Info(request.Text);
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Trace(TraceRequest request, ServerCallContext context)
        {
            _log.Trace(request.Trace);
            return Task.FromResult(new Empty());
        }
    }
}