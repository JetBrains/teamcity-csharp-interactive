namespace TeamCity.CSharpInteractive
{
    using System.Diagnostics;
    using Cmd;

    internal interface IStartInfoFactory
    {
        ProcessStartInfo Create(IStartInfo info);
    }
}