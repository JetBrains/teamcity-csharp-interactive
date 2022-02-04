static class Tools
{
    public static string GetProperty(string name, string defaultProp, bool showWarning = false)
    {
        var prop = Props[name];
        if (!string.IsNullOrWhiteSpace(prop))
        {
            return prop;
        }

        var message = $"The property \"{name}\" was not defined, the default value \"{defaultProp}\" was used.";
        if (showWarning)
        {
            Warning(message);
        }
        else
        {
            Info(message);
        }

        return defaultProp;
    }
}