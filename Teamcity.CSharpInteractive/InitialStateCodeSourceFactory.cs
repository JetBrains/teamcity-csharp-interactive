// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    [ExcludeFromCodeCoverage]
    internal class InitialStateCodeSourceFactory : IInitialStateCodeSourceFactory
    {
        private readonly Func<InitialStateCodeSource> _codeSourceFactory;

        public InitialStateCodeSourceFactory(
            Func<InitialStateCodeSource> codeSourceFactory) =>
            _codeSourceFactory = codeSourceFactory;

        public ICodeSource Create(IReadOnlyCollection<string> scriptArguments, IReadOnlyDictionary<string, string> scriptProperties)
        {
            var codeSource = _codeSourceFactory();
            codeSource.ScriptArguments = scriptArguments;
            codeSource.ScriptProperties = scriptProperties;
            return codeSource;
        }
    }
}