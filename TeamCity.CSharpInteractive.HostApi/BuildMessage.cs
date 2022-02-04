// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedPositionalProperty.Global
namespace HostApi;

using Immutype;
using JetBrains.TeamCity.ServiceMessages;

[Target]
public readonly record struct BuildMessage(
    BuildMessageState State,
    IServiceMessage? ServiceMessage = default,
    string Text = "",
    string ErrorDetails = "",
    string Code = "",
    string File = "",
    string Subcategory = "",
    string ProjectFile = "",
    string SenderName = "",
    int? ColumnNumber = default,
    int? EndColumnNumber = default,
    int? LineNumber = default,
    int? EndLineNumber = default,
    DotNetMessageImportance? Importance = default)
{
    public override string ToString() => Text;
}