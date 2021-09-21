namespace TeamCity.CSharpInteractive
{
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    internal class TargetFrameworkMonikerParser : ITargetFrameworkMonikerParser
    {
        private static readonly Regex NetFull = new Regex("^net\\d+$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static readonly Regex Net = new Regex("^net[\\d\\.]+$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static readonly Regex NetCore = new Regex("^netcoreapp[\\d\\.]+$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static readonly Regex NetStandard = new Regex("^netstandard[\\d\\.]+$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        private static readonly Regex Uap = new Regex("^uap[\\d\\.]+$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);
        
        public string Parse(string tfm)
        {
            tfm = tfm.Trim();
            if (string.IsNullOrWhiteSpace(tfm))
            {
                return tfm;
            }

            if (NetFull.IsMatch(tfm))
            {
                var version = string.Join(".", tfm.Substring(3, tfm.Length - 3).Select(i => $"{i}"));
                return $".NETFramework,Version=v{version}";
            }
            
            if (Net.IsMatch(tfm))
            {
                var version = tfm.Substring(3, tfm.Length - 3);
                return $".NETCoreApp,Version=v{version}";
            }
            
            if (NetCore.IsMatch(tfm))
            {
                var version = tfm.Substring(10, tfm.Length - 10);
                return $".NETCoreApp,Version=v{version}";
            }
            
            if (NetStandard.IsMatch(tfm))
            {
                var version = tfm.Substring(11, tfm.Length - 11);
                return $".NETStandard,Version=v{version}";
            }
            
            if (Uap.IsMatch(tfm))
            {
                var version = tfm.Substring(3, tfm.Length - 3);
                return $"UAP,Version=v{version}";
            }

            return tfm;
        }
    }
}