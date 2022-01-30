// ReSharper disable UnusedMember.Global
namespace TeamCity.CSharpInteractive;

internal interface IInfo
{
    void ShowHeader();

    void ShowReplHelp();

    void ShowHelp();

    void ShowVersion();

    void ShowFooter();
}