using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public class Variable : IExpression
    {
        public string name;
        public TokenType type;
        public object value;
        public Variable(string name, TokenType type)
        {
            this.name = name;
            this.type = type;
        }

        public Variable(string name, TokenType type, object value) : this(name, type)
        {
            this.value = value;
        }

        public Variable(DeclarationStatement statement)
        {
            this.name = statement.name;
            this.type = statement.variableType;
            this.value = null;
        }

        public object Evaluate(Scope scope)
        {
            return null;
        }
    }
}
