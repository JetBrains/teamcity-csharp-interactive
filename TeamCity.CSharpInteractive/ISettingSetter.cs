namespace TeamCity.CSharpInteractive;

internal interface ISettingSetter<TSetting>
    where TSetting: struct, Enum
{
    TSetting SetSetting(TSetting value);
}