// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Contracts;
    using Microsoft.CodeAnalysis.Scripting;

    [ExcludeFromCodeCoverage]
    internal class ScriptStatePresenter: IPresenter<ScriptState<object>>
    {
        private const string Tab = " ";
        private static readonly IEnumerable<Text> EmptyLine = new []{Text.NewLine };
        private readonly ILog<ScriptStatePresenter> _log;
        private readonly IProperties _properties;

        public ScriptStatePresenter(ILog<ScriptStatePresenter> log, IProperties properties)
        {
            _log = log;
            _properties = properties;
        }

        public void Show(ScriptState<object> data)
        {
            if (data.Variables.Any())
            {
                var vars = GetTrace(
                    "Variables",
                    (
                        from variable in data.Variables
                        group variable by variable.Name
                        into gr
                        select gr.Last())
                    .Select(GetVariablyTrace));
                
                var props = GetTrace(
                    "Properties",
                    _properties.Select(i => $"{Tab}Props[\"{i.Key}\"] = \"{i.Value}\""));

                _log.Trace(
                    EmptyLine
                        .Concat(vars)
                        .Concat(EmptyLine)
                        .Concat(props).ToArray());
            }
            else
            {
                _log.Trace("No variables defined.");
            }
        }

        private static IEnumerable<Text> GetTrace(string name, IEnumerable<string> items) => 
            new [] {new Text($"{name}:")}.Concat(items.Select(i => new [] {Text.NewLine, new Text($"{Tab}{i}")}).SelectMany(i => i));

        private static string GetVariablyTrace(ScriptVariable variable)
        {
            var sb = new StringBuilder(Tab);
            if (variable.IsReadOnly)
            {
                sb.Append("readonly ");
            }

            sb.Append(variable.Type.Name);
            sb.Append(' ');
            sb.Append(variable.Name);
            sb.Append(" = ");
            sb.Append(variable.Value);
            return sb.ToString();
        }
    }
}