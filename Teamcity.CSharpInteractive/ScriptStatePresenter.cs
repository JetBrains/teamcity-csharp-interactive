// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
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
            var trace = new List<Text>();
            if (data.Variables.Any())
            {
                trace.Add(new Text("Variables"));
                var variables = 
                    from variable in data.Variables
                    group variable by variable.Name
                    into gr
                    select gr.Last();
                
                trace.AddRange(variables.SelectMany(GetVariablyTrace));
            }

            _log.Trace(trace.ToArray());
        }
        
        private static Text[] GetVariablyTrace(ScriptVariable variable)
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

            return new[] {Text.NewLine, new Text(sb.ToString())};
        }
    }
}