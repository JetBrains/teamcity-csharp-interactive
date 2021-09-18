namespace TeamCity.CSharpInteractive
{
    using System;

    internal interface ISettingDescription
    {
        bool IsVisible { get; }
        
        Type SettingType { get; }
            
        string Key { get; }

        string Description { get; }
    }
}