// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive.Contracts
{
    using System.Collections.Generic;

    public interface IHost
    {
        IHost Host { get; }
        
        IReadOnlyList<string> Args { get; }
        
        IReadOnlyDictionary<string, string> Props { get; }

        void WriteLine();
        
        void WriteLine<T>(T line, Color color = Color.Default);
        
        void Error(string error, string errorId = "Unknown");
        
        void Warning(string warning);
        
        void Info(string text);
        
        void Trace(string trace, string origin = "");
        
        T GetService<T>();
    }
}