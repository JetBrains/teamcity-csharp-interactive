namespace TeamCity.CSharpInteractive;

using System.Diagnostics;
using Script.Cmd;

internal interface IStartInfoFactory
{
    ProcessStartInfo Create(IStartInfo info);
}