// ReSharper disable UnusedMember.Global
namespace Teamcity.CSharpInteractive.Contracts
{
    public interface IHost
    {
        void WriteLine();
        
        void WriteLine<T>(T line, Color color = Color.Default);
        
        void Error(string error, string errorId = "Unknown");
        
        void Warning(string warning);
        
        void Info(string text);
        
        void Trace(string trace, string origin = "");
    }
}