namespace Teamcity.CSharpInteractive
{
    using System;

    internal class VerbosityLevelSettingDescription : ISettingDescription
    {
        public Type SettingType => typeof(VerbosityLevel);

        public string Key => "l";

        public string Description => "Set a verbosity level";
    }
}