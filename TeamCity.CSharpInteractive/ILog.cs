// ReSharper disable UnusedTypeParameter

namespace TeamCity.CSharpInteractive
{
    using System;

    internal interface ILog<T>
    {
        void Error(ErrorId id, params Text[] error);
        
        void Warning(params Text[] warning);
        
        void Info(params Text[] message);

        void Trace(Func<Text[]> traceMessagesFactory, string origin = "");
    }
}