using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logo.Core.Utils;
using Logo.Core.Utils.Grammar;
using System.Drawing;

namespace Logo.Core
{
    public class Scope
    {
        Dictionary<string, Variable> variables = new Dictionary<string, Variable>();
        public Scope()
        {

        }

        public bool contains(string name)
        {
            return variables.ContainsKey(name);
        }

        public bool contains(string[] name)
        {
            Dictionary<string, object> sub = null;
            for (int i = 0; i < variables.Count; i++)
            {
                if (i == 0) 
                    if (!variables.ContainsKey(name[i]))
                        return false;
                    else if (variables[name[i]].value is Dictionary<string, object>)
                        sub = (Dictionary<string, object>)variables[name[i]].value;
                if (i != 0)
                    if (!sub.ContainsKey(name[i]))
                        return false;
                    else if (sub[name[i]] is Dictionary<string, object>)
                        sub = (Dictionary<string, object>)sub[name[i]];
            }
            return true;
        }

        public void putVariable(Variable variable)
        {
            variables[variable.name] = variable;
        }

        public Variable getVariable(string name)
        {
            if (!contains(name))
            {
                return null;
            }
            return variables[name];
        }

        public void setVariable(string name, Variable value)
        {
            variables[name] = value;
        }

        public void setVariableValue(string name, object value)
        {
            if (contains(name))
            {
                variables[name].value = value;
            }
        }
    }

    public class FunctionStorage
    {
        public static Dictionary<string, FunctionStatement> functions = new Dictionary<string, FunctionStatement>();
        
        public static void setFunction(string name, FunctionStatement function)
        {
            functions[name] = function;
        }

        public static FunctionStatement getFunction(string name)
        {
            if (!functions.ContainsKey(name))
                return null;
            return functions[name];
        }

        public static void setFunctions(Dictionary<string, FunctionStatement> functionList)
        {
            functions = functionList;
        }
    }

    public class Interpreter
    {
        public Bitmap result { get; private set; } = null;
        public Interpreter() { }
        public object Run(Dictionary<string, FunctionStatement> functions, Header header = null)
        {
            FunctionStorage.setFunctions(functions);
            if (!functions.ContainsKey("main")) {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Interpreter Error: No main function found!"));
                return null;
            }
            Scope emptyScope = new Scope();
            if (header != null)
            {
                emptyScope.setVariable(
                    "Board", 
                    new Variable("Board", VariableType.BOARD, new Board(header.width, header.height))
                );
            }
            var obj = functions["main"].Execute(emptyScope);
            if (header != null)
            {
                Variable variable = emptyScope.getVariable("Board");
                if (variable != null)
                {
                    Board b = (Board)variable.value;
                    result = b.bitmap;
                }
            }
            return obj;
        }
    }
}
