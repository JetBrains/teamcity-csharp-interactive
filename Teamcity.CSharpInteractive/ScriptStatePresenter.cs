// ReSharper disable ClassNeverInstantiated.Global
namespace Teamcity.CSharpInteractive
{
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
                _log.Trace("Variables:");
                var variables = 
                    from variable in data.Variables
                    group variable by variable.Name
                    into gr
                    select gr.Last();

                _log.Trace(string.Join(System.Environment.NewLine, variables.Select(GetVariablyTrace)));
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