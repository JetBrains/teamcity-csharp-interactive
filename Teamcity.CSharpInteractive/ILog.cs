// ReSharper disable UnusedTypeParameter

namespace Teamcity.CSharpInteractive
{
    using System;

    internal interface ILog<T>
    {
        void Error(ErrorId id, params Text[] error);
        
        void Warning(params Text[] warning);
        
        void Info(params Text[] message);

        void Trace(string origin, params Text[] traceMessage);

        IDisposable Block(Text[] block);
    }
}