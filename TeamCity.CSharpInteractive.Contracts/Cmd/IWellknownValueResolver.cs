// ReSharper disable CheckNamespace
namespace Cmd
{
    internal interface IWellknownValueResolver
    {
        string Resolve(WellknownValue value);
    }
}