// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedPositionalProperty.Global
namespace Dotnet
{
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages;

    public readonly record struct BuildMessage(
        BuildMessageState State,
        IReadOnlyList<IServiceMessage> ServiceMessages,
        string Text = "",
        string ErrorDetails = "");
}