// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using Microsoft.CodeAnalysis.CSharp;

    internal class LanguageVersionSettingDescription : ISettingDescription
    {
        public bool IsVisible => true;

        public Type SettingType => typeof(LanguageVersion);

        public string Key => "lv";

        public string Description => "Set a C# language version";
    }
}