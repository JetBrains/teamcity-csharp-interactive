namespace Script.Cmd;

internal interface IVirtualContext
{
    string Resolve(string path);
}