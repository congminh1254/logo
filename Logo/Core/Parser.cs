using Logo.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logo.Core
{
    internal class Parser
    {
        Lexer lexer;
        Dictionary<String, FunctionStatement> functions;
        Token currentToken, nextToken;
        public Parser(Lexer lexer) {
            this.lexer= lexer;

        }

    }
}
