// ReSharper disable ClassNeverInstantiated.Global
namespace TeamCity.CSharpInteractive
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Microsoft.CodeAnalysis.Scripting;

    [ExcludeFromCodeCoverage]
    internal class ScriptStatePresenter: IPresenter<ScriptState<object>>
    {
        private readonly ILog<ScriptStatePresenter> _log;

        public ScriptStatePresenter(ILog<ScriptStatePresenter> log) => _log = log;

        public void Show(ScriptState<object> data)
        {
            if (data.Variables.Any())
            {
                var trace = new List<Text> {new("Variables:")};
                var variables = (
                    from variable in data.Variables
                    group variable by variable.Name
                    into gr
                    select gr.Last())
                    .Select(GetVariablyTrace)
                    .Select(i => new [] {Text.NewLine, new Text($"  {i}")})
                    .SelectMany(i => i);

                trace.AddRange(variables);
                _log.Trace(trace.ToArray());
            }
            else
            {
                _log.Trace("No variables defined.");
            }
        }
        
        private static string GetVariablyTrace(ScriptVariable variable)
        {
            var sb = new StringBuilder("  ");
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