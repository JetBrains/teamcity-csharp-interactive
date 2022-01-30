namespace TeamCity.CSharpInteractive;

internal class Setting<T> : ISettingGetter<T>, ISettingSetter<T>
    where T: struct, Enum
{
    private T _settingValue;

    public Setting(T defaultSettingValue) => _settingValue = defaultSettingValue;

    public T GetSetting() => _settingValue;

    public T SetSetting(T value)
    {
        var prevValue = _settingValue;
        _settingValue = value;
        return prevValue;
    }
}