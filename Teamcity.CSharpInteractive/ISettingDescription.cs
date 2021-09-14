namespace Teamcity.CSharpInteractive
{
    using System;

    internal interface ISettingDescription
    {
        Type SettingType { get; }
            
        string Key { get; }

        string Description { get; }
    }
}