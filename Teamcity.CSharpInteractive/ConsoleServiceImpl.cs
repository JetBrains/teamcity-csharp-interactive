// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Diagnostics.CodeAnalysis;
    using System.Threading.Tasks;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Host;

    [ExcludeFromCodeCoverage]
    internal class ConsoleServiceImpl: ConsoleService.ConsoleServiceBase
    {
        private readonly IStdOut _stdOut;

        public ConsoleServiceImpl(IStdOut stdOut) => _stdOut = stdOut;

        public override Task<Empty> WriteLine(WriteLineRequest request, ServerCallContext context)
        {
            _stdOut.WriteLine(new Text(request.Line, request.Color));
            return Task.FromResult(new Empty());
        }
    }
}