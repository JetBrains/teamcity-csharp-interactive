namespace TeamCity.CSharpInteractive.Contracts
{
    using System.Collections.Generic;

    public interface IProperties: IEnumerable<KeyValuePair<string, string>>
    {
        int Count { get; }

        string this[string key] { get; set; }

        bool TryGetValue(string key, out string value);
    }
}