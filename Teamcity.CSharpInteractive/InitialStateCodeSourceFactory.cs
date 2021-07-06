namespace Teamcity.CSharpInteractive
{
    using System;
    using System.Collections.Generic;

    internal class InitialStateCodeSourceFactory : IInitialStateCodeSourceFactory
    {
        private readonly Func<InitialStateCodeSource> _codeSourceFactory;

        public InitialStateCodeSourceFactory(
            Func<InitialStateCodeSource> codeSourceFactory)
        {
            _codeSourceFactory = codeSourceFactory;
        }

        public ICodeSource Create(IReadOnlyCollection<string> scriptArguments)
        {
            var codeSource = _codeSourceFactory();
            codeSource.ScriptArguments = scriptArguments;
            return codeSource;
        }
    }
}