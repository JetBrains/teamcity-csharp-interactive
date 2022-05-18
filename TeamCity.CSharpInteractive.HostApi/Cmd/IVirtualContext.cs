namespace HostApi.Cmd;

internal interface IVirtualContext
{
    bool IsActive { get; }
    
    string Resolve(string path);
}