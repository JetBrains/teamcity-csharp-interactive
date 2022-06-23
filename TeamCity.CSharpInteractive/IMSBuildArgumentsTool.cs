// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive;

internal interface IMSBuildArgumentsTool
{
    string Unescape(string escaped);
}