// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Contracts;

    internal class Properties: IProperties
    {
        private readonly Dictionary<string, string> _props;
        
        public Properties(ISettings settings) => 
            _props = new Dictionary<string, string>(FilterPairs(settings.ScriptProperties));

        public int Count
        {
            get
            {
                lock (_props)
                {
                    return _props.Count;
                }
            }
        }

        public string this[string key]
        {
            get => TryGetValue(key, out var value) ? value : string.Empty;
            set
            {
                lock (_props)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        _props[key] = value;
                    }
                    else
                    {
                        _props.Remove(key);
                    }
                }
            }
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            IReadOnlyList<KeyValuePair<string, string>> props;
            lock (_props)
            {
                props = FilterPairs(_props).ToList().AsReadOnly();
            }

            return props.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public bool TryGetValue(string key, out string value)
        {
            lock (_props)
            {
                _props.TryGetValue(key, out var curValue);
                value = curValue ?? string.Empty;
                return !string.IsNullOrEmpty(value);
            }
        }

        private static IEnumerable<KeyValuePair<string, string>> FilterPairs(IEnumerable<KeyValuePair<string, string>> pairs) =>
            pairs.Where(i => !string.IsNullOrWhiteSpace(i.Key) && !string.IsNullOrEmpty(i.Value));
    }
}