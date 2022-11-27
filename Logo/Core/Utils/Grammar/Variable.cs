using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public class Variable: Expression
    {
        public string name;
        public string type;
        public object value;
        public Variable(string name, string type) {
            this.name = name;
            this.type = type;
        }

        public Variable(string name, string type, object value) : this(name, type)
        {
            this.value = value;
        }

        public Variable(DeclarationStatement statement)
        {
            this.name = statement.name;
            this.type = statement.variableType;
            this.value = null;
        }

        public override object evaluate(Scope scope)
        {
            return null;
        }
    }
}
