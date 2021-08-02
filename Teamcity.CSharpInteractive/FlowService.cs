// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Threading.Tasks;
    using Google.Protobuf.WellKnownTypes;
    using Grpc.Core;
    using Host;

    internal class FlowService: Flow.FlowBase, IFlow
    {
        public event Action? OnCompleted;

        public override Task<Empty> Completed(Empty request, ServerCallContext context)
        {
            OnCompleted?.Invoke();
            return Task.FromResult(request);
        }
    }
}