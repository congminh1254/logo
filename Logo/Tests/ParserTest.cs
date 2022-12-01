using Logo;
using Logo.Core;
using Logo.Core.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using Logo.Core.Utils.Grammar;

namespace Logo.Tests
{
    [TestFixture]
    public class ParserTest
    {
        [Test]
        public void parseFunctionTest()
        {
            string code = "add(a: int, b: int)\n" +
                "{\n" +
                "   return a+b\n" +
                "}\n";
            Lexer lexer = new Lexer(new SourceCode(code, false));
            Parser parser = new Parser(lexer);

            Dictionary<string, FunctionStatement> result = parser.parse();
            Assert.NotNull(result);
            Assert.AreNotEqual(result.Count, 0);
        }
    }
}
