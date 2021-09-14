namespace Teamcity.CSharpInteractive
{
    using System;
    using Microsoft.CodeAnalysis;

    internal class OptimizationLevelSettingDescription : ISettingDescription
    {
        public Type SettingType => typeof(OptimizationLevel);

        public string Key => "ol";

        public string Description => "Set an optimization level";
    }
}