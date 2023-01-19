using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public enum VariableType
    {
        INT,
        FLOAT,
        STR,
        TURTLE,
        BOOL,
        TURTLE_PEN,
        COORDINATE,
        COLOR,
        BOARD
    }
    public class Variable: IExpression
    {
        public object value { get; set; }

        public Variable(object value)
        {
            this.value = value;
        }

        public object Evaluate(Scope scope)
        {
            return value;
        }
    }
}
