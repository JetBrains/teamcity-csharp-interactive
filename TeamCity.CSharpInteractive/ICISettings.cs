// ReSharper disable UnusedMember.Global
// ReSharper disable InconsistentNaming
namespace TeamCity.CSharpInteractive;

internal interface ICISettings
{
    CIType CIType { get; }

    string Version { get; }

    string FlowId { get; }

    string ServiceMessagesPath { get; }
}