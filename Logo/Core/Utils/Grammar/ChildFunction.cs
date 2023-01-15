using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core.Utils.Grammar
{
    public class ChildFunction
    {
        public object parent;
        public string name;
        public List<DeclarationStatement> parameters;
        public ChildFunction(object parent, string name, List<DeclarationStatement> parameters) 
        {
            this.parent = parent;
            this.name = name;
            this.parameters = parameters;
        }

        public object invoke(Scope scope)
        {
            if (parent is TurtleVar)
                return ((TurtleVar)parent).invoke(name, scope);
            return null;
        }
    }
}
