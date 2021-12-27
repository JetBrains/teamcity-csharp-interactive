// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedPositionalProperty.Global
namespace Dotnet
{
    using JetBrains.TeamCity.ServiceMessages;

    [Immutype.Target]
    public readonly record struct BuildMessage(
        BuildMessageState State,
        IServiceMessage? ServiceMessage = default,
        string Text = "",
        string ErrorDetails = "");
}