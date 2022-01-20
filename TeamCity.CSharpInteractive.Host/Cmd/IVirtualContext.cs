// ReSharper disable CheckNamespace
namespace Cmd;

internal interface IVirtualContext
{
    string Resolve(string path);
}