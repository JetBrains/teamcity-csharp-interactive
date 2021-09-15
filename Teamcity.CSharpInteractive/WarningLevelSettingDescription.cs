// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;

    internal class WarningLevelSettingDescription : ISettingDescription
    {
        public bool IsVisible => true;

        public Type SettingType => typeof(WarningLevel);

        public string Key => "wl";

        public string Description => "Set a warning level";
    }
}