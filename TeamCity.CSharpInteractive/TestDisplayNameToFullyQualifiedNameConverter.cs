// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System;
    using System.Linq;

    internal class TestDisplayNameToFullyQualifiedNameConverter : ITestDisplayNameToFullyQualifiedNameConverter
    {
        private static readonly string[] ArgsStartPrefix = { "(", "<" };
        
        public string Convert(string displayName)
        {
            if (!displayName.TrimEnd().EndsWith(")"))
            {
                return displayName;
            }

            var startArgsPosition = (
                    from prefix in ArgsStartPrefix
                    let position = displayName.IndexOf(prefix, StringComparison.Ordinal)
                    where position >= 0
                    select position)
                .DefaultIfEmpty(-1)
                .Min();

            var result = startArgsPosition <= 0 ? displayName : displayName[..startArgsPosition].Trim();
            return !string.IsNullOrWhiteSpace(result) ? result : displayName;
        }
    }
}