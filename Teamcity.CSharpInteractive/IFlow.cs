namespace Teamcity.CSharpInteractive
{
    using System;

    internal interface IFlow
    {
        public event Action? OnCompleted;
    }
}