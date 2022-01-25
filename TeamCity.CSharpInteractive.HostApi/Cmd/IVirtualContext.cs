namespace HostApi.Cmd;

internal interface IVirtualContext
{
    string Resolve(string path);
}