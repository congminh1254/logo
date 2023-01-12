using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logo.Core.Utils;
using Logo.Core.Utils.Grammar;

namespace Logo.Core
{
    public class Scope
    {
        Dictionary<String, Variable> variables = new Dictionary<string, Variable>();
        public Scope()
        {

        }

        public bool contains(string name)
        {
            return variables.ContainsKey(name);
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
            return functions[name];
        }

        public static void setFunctions(Dictionary<string, FunctionStatement> functionList)
        {
            functions = functionList;
        }
    }

    public class Interpreter
    {
        public static Object Run(Dictionary<string, FunctionStatement> functions)
        {
            FunctionStorage.setFunctions(functions);
            if (!functions.ContainsKey("main")) {
                ErrorHandling.pushError(new ErrorHandling.LogoException("Interpreter Error: No main function found!"));
                return null;
            }
            return functions["main"].Execute(null);
        }
    }
}
