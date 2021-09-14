namespace Teamcity.CSharpInteractive
{
    using System;

    internal interface ISettingSetter<TSetting>
        where TSetting: struct, Enum
    {
        TSetting? SetSetting(TSetting value);
    }
}