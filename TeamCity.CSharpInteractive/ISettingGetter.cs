namespace TeamCity.CSharpInteractive
{
    using System;

    internal interface ISettingGetter<out TSetting>
        where TSetting: struct, Enum
    {
        TSetting GetSetting();
    }
}