// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;

    internal class CheckOverflowSettingDescription : ISettingDescription
    {
        public bool IsVisible => true;

        public Type SettingType => typeof(CheckOverflow);

        public string Key => "co";

        public string Description => "Enable or disable overflow check";
    }
}