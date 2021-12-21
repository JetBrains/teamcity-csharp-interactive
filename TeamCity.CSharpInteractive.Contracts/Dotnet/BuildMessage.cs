// ReSharper disable CheckNamespace
// ReSharper disable InconsistentNaming
// ReSharper disable NotAccessedPositionalProperty.Global
namespace Dotnet
{
    using System.Collections.Generic;
    using JetBrains.TeamCity.ServiceMessages;

    public record BuildMessage(
        BuildMessageState State,
        IEnumerable<IServiceMessage> ServiceMessages,
        string Text = "",
        string ErrorDetails = "");
}