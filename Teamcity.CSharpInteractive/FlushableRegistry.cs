// ReSharper disable IdentifierTypo
// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System.Collections.Generic;

    internal class FlushableRegistry : IFlushableRegistry, IFlushable
    {
        private readonly List<IFlushable> _flushables = new();
        
        public void Register(IFlushable flushable) => _flushables.Add(flushable);

        public void Flush()
        {
            try
            {
                foreach (var flushable in _flushables)
                {
                    flushable.Flush();
                }
            }
            finally
            {
                _flushables.Clear();
            }
        }
    }
}