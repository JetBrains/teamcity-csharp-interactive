// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class WarningLevelSettingDescription : ISettingDescription
    {
        public bool IsVisible => false;

        public Type SettingType => typeof(WarningLevel);

        public string Key => "wl";

        public string Description => "Set a warning level";
    }
}