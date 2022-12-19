using Logo;
using Logo.Core;
using Logo.Core.Utils;
using System.Collections.Generic;
using NUnit.Framework;
using System;
using System.IO;
using NUnit.Framework.Internal;
using Logo.Core.Utils.Grammar;
using NUnit.Framework.Constraints;

namespace Logo
{
    [TestFixture]
    public class ParserTest
    {
        public List<IStatement> ParseCode(string code)
        {
            string example = "main() {\r\n " + code + "\r\n}\r\n";
            SourceCode source = new SourceCode(Utils.stringToStreamReader(example));
            Lexer lexer = new Lexer(source);
            Parser parser = new Parser(lexer);

            var result = parser.parse();
            return result["main"].body;
        }

        [Test]
        public void TestParseFunction()
        {
            string code = "plus(a: int, b: float) {\r\n   return a+b\r\n}\r\n";
            Lexer lexer = new Lexer(new SourceCode(Utils.stringToStreamReader(code)));
            Parser parser = new Parser(lexer);

            var result = parser.parse();
            Assert.NotZero(result.Count);
            var func = result["plus"];
            Assert.IsTrue(func is FunctionStatement);
            var identifier = func.identifier;
            var parameters = func.parameters;
            var body = func.body;
            Assert.AreEqual(identifier.textValue, "plus");
            Assert.AreEqual(parameters[0].name, "a");
            Assert.AreEqual(parameters[1].name, "b");
            Assert.AreEqual(parameters[0].variableType, TokenType.INT_T);
            Assert.AreEqual(parameters[1].variableType, TokenType.FLOAT_T);

            Assert.AreEqual(body.Count, 1);
            Assert.IsTrue(body[0] is ReturnStatement);

            var ret = (ReturnStatement)body[0];
            Assert.IsTrue(ret.expression is Sum);
            var exp = (Sum)ret.expression;
            Assert.IsTrue(exp.left is Identifier);
            Assert.IsTrue(exp.right is Identifier);
        }

        [Test]
        public void TestParseIf()
        {
            string code = "if a>3 {\r\n\ta = a+b\r\n} else {\r\n\treturn a+b\r\n}";
            var statements = ParseCode(code);

            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];

            Assert.IsTrue(statement is IfStatement);
            var ifstatement = (IfStatement)statement;
            var cond = ifstatement.condition;
            var body = ifstatement.body;
            var elsebody = ifstatement.elseBody;
            Assert.IsTrue(cond is Comparison);
            Assert.IsTrue(((Comparison)cond).left is Identifier);
            Assert.IsTrue(((Comparison)cond).right is Literal);

            Assert.AreEqual(body.Count, 1);
            Assert.AreEqual(elsebody.Count, 1);
            Assert.IsTrue(body[0] is AssignStatement);
            Assert.IsTrue(elsebody[0] is ReturnStatement);

            AssignStatement stat1 = (AssignStatement)body[0];
            Assert.AreEqual(stat1.variable, "a");
            Assert.IsTrue(stat1.expression is Sum);
            ReturnStatement stat2 = (ReturnStatement)elsebody[0];
            Assert.IsTrue(stat2.expression is Sum);
        }

        [Test]
        public void TestParseWhile()
        {
            string code = "while a > 3 {\r\n\ta = a-1\r\n}";
            var statements = ParseCode(code);

            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is WhileStatement);

            Assert.AreEqual(((WhileStatement)statement).body.Count, 1);
        }

        [Test]
        public void TestParseFunctionCall()
        {
            string code = "print(a, 3+5)";
            var statements = ParseCode(code);

            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is FunctionCallStatement);
            Assert.AreEqual(((FunctionCallStatement)statement).arguments.Count, 2);
            Assert.IsTrue(((FunctionCallStatement)statement).arguments[0] is Identifier);
            Assert.IsTrue(((FunctionCallStatement)statement).arguments[1] is Sum);
        }

        [Test]
        public void TestParseCondition()
        {
            string code = "if a<b AND c==d OR NOT (a > 0 OR a > 3) {\r\nreturn 0\r\n}\r\n";
            var statements = ParseCode(code);
            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is IfStatement);
            Assert.IsTrue(((IfStatement)statement).condition is Or);

            Or condition = (Or)((IfStatement)statement).condition;
            Binary cl = (Binary)condition.left;
            Unary cr = (Unary)condition.right;
            Assert.IsTrue(cl is AndExpresstion);
            Assert.IsTrue(cr is NotExpression);
            Assert.AreEqual(condition.op.tokenType, TokenType.OR);
            Binary cll = (Binary)cl.left;
            Binary clr = (Binary)cl.right;
            Assert.IsTrue(cll is Comparison);
            Assert.IsTrue(clr is Equality);
            Binary crr = (Binary)cr.right;
            Assert.IsTrue(crr is Or);
            Binary crrl = (Binary)crr.left;
            Binary crrr = (Binary)crr.right;
            Assert.IsTrue(crrl is Comparison);
            Assert.IsTrue(crrr is Comparison);
        }

        [Test]
        public void TestParseDuplicatedFunction()
        {
            string code = "main() {\r\n}\r\nmain() {\r\n}\r\n";
            Lexer lexer = new Lexer(new SourceCode(Utils.stringToStreamReader(code)));
            Parser parser = new Parser(lexer);

            parser.parse();
            Assert.AreEqual(ErrorHandling.exceptions.Count, 1);
        }

        [Test]
        public void TestParseMathematic()
        {
            string code = "a = (2.5+3*5-1)/3%5";
            var statements = ParseCode(code);
            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is AssignStatement);
            Binary op = (Binary)((AssignStatement)statement).expression;
            Binary ol = (Binary)op.left;
            Assert.IsTrue(op is Modulo);
            Assert.IsTrue(ol is Division);
            Assert.IsTrue(op.right is Literal);

            Binary oll = (Binary)ol.left;
            Binary olll = (Binary)oll.left;
            Assert.IsTrue(oll is Subtract);
            Assert.IsTrue(oll.left is Sum);
            Assert.IsTrue(oll.right is Literal);

            Assert.IsTrue(olll is Sum);
            Assert.IsTrue(olll.left is Literal);
            Assert.IsTrue(olll.right is Multiplication);
        }

        [Test]
        public void TestCreateTurtle()
        {
            string code = "turtle = Turtle()";
            var statements = ParseCode(code);

            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is AssignStatement);
            Literal l = ((AssignStatement)statement).expression as Literal;
            Assert.IsTrue(l.value is TurtleVar);
        }

        [Test]
        public void TestParseFunctionCallExp()
        {
            string code = "a = func(3, 2, true, false, \"abc\")";
            var statements = ParseCode(code);

            Assert.AreEqual(statements.Count, 1);
            Assert.IsTrue(statements[0] is AssignStatement);
            var statement = (AssignStatement)statements[0];
            Assert.IsTrue(statement.expression is FunctionCallExp);
            var exp = (FunctionCallExp)statement.expression;
            Assert.AreEqual(exp.identifier, "func");
            Assert.AreEqual(exp.arguments.Count, 5);
        }
    }
}
