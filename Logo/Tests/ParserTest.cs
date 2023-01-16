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
            return ((BlockStatement)result["main"].body).statements;
        }

        [Test]
        public void TestParseFunction()
        {
            ErrorHandling.clear();
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
            Assert.AreEqual(parameters[0].variableType, VariableType.INT);
            Assert.AreEqual(parameters[1].variableType, VariableType.FLOAT);

            Assert.AreEqual(((BlockStatement)body).statements.Count, 1);
            Assert.IsTrue(((BlockStatement)body).statements[0] is ReturnStatement);

            var ret = (ReturnStatement)((BlockStatement)body).statements[0];
            Assert.IsTrue(ret.expression is Sum);
            var exp = (Sum)ret.expression;
            Assert.IsTrue(exp.left is Identifier);
            Assert.IsTrue(exp.right is Identifier);
        }

        [Test]
        public void TestParseIf()
        {
            ErrorHandling.clear();
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

            Assert.AreEqual(((BlockStatement)body).statements.Count, 1);
            Assert.AreEqual(((BlockStatement)elsebody).statements.Count, 1);
            Assert.IsTrue(((BlockStatement)body).statements[0] is AssignStatement);
            Assert.IsTrue(((BlockStatement)elsebody).statements[0] is ReturnStatement);

            AssignStatement stat1 = (AssignStatement)((BlockStatement)body).statements[0];
            Assert.AreEqual(stat1.attr.variableName, "a");
            Assert.IsTrue(stat1.expression is Sum);
            ReturnStatement stat2 = (ReturnStatement)((BlockStatement)elsebody).statements[0];
            Assert.IsTrue(stat2.expression is Sum);
        }

        [Test]
        public void TestParseWhile()
        {
            ErrorHandling.clear();
            string code = "while a > 3 {\r\n\ta = a-1\r\n}";
            var statements = ParseCode(code);

            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is WhileStatement);

            Assert.AreEqual(((BlockStatement)((WhileStatement)statement).body).statements.Count, 1);
        }

        [Test]
        public void TestParseFunctionCall()
        {
            ErrorHandling.clear();
            string code = "print(a, 3+5)";
            var statements = ParseCode(code);

            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is FunctionCallStatement);
        }

        [Test]
        public void TestParseCondition()
        {
            ErrorHandling.clear();
            string code = "if a<b AND c==d OR NOT (a > 0 OR a > 3) {\r\nreturn 0\r\n}\r\n";
            var statements = ParseCode(code);
            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is IfStatement);
            Assert.IsTrue(((IfStatement)statement).condition is Or);

            Or condition = (Or)((IfStatement)statement).condition;
            Assert.IsTrue(condition.left is AndExpresstion);
            Assert.IsTrue(condition.right is NotExpression);
            AndExpresstion cl = (AndExpresstion)condition.left;
            NotExpression cr = (NotExpression)condition.right;

            Assert.IsTrue(cl.left is Comparison);
            Assert.IsTrue(cl.right is Equality);
            Comparison cll = (Comparison)cl.left;
            Equality clr = (Equality)cl.right;

            Assert.IsTrue(cr.right is Or);
            Or crr = (Or)cr.right;

            Assert.IsTrue(crr.left is Comparison);
            Assert.IsTrue(crr.right is Comparison);
            Comparison crrl = (Comparison)crr.left;
            Comparison crrr = (Comparison)crr.right;
        }

        [Test]
        public void TestParseDuplicatedFunction()
        {
            ErrorHandling.clear();
            string code = "main() {\r\n}\r\nmain() {\r\n}\r\n";
            Lexer lexer = new Lexer(new SourceCode(Utils.stringToStreamReader(code)));
            Parser parser = new Parser(lexer);

            parser.parse();
            Assert.AreEqual(ErrorHandling.exceptions.Count, 1);
        }

        [Test]
        public void TestParseCopyVariable()
        {
            ErrorHandling.clear();
            string code = "a = copyof b";
            var statements = ParseCode(code);
            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is AssignStatement);

            AssignStatement assign = (AssignStatement)statement;
            Assert.IsTrue(assign.expression is CopyOfExp);
            Assert.AreEqual(ErrorHandling.exceptions.Count, 0);
        }

        [Test]
        public void TestParseMathematic()
        {
            ErrorHandling.clear();
            string code = "a = (2.5+3*5-1)/3%5";
            var statements = ParseCode(code);
            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is AssignStatement);

            Assert.IsTrue(((AssignStatement)statement).expression is Modulo);
            Modulo op = (Modulo)((AssignStatement)statement).expression;
            Assert.IsTrue(op.left is Division);
            Division ol = (Division)op.left;
            Assert.IsTrue(op.right is Literal);

            Assert.IsTrue(ol.left is Subtract);
            Subtract oll = (Subtract)ol.left;
            Assert.IsTrue(oll.left is Sum);
            Assert.IsTrue(oll.right is Literal);

            Assert.IsTrue(oll.left is Sum);
            Sum olll = (Sum)oll.left;
            Assert.IsTrue(olll.left is Literal);
            Assert.IsTrue(olll.right is Multiplication);
            Assert.AreEqual(ErrorHandling.exceptions.Count, 0);
        }

        [Test]
        public void TestCreateTurtle()
        {
            ErrorHandling.clear();
            string code = "turtle = Turtle()";
            var statements = ParseCode(code);

            Assert.AreEqual(statements.Count, 1);
            var statement = statements[0];
            Assert.IsTrue(statement is AssignStatement);
            FunctionCallExp l = ((AssignStatement)statement).expression as FunctionCallExp;
            Assert.AreEqual(l.attr.variableName, "Turtle");
            Assert.AreEqual(ErrorHandling.exceptions.Count, 0);
        }

        [Test]
        public void TestParseFunctionCallExp()
        {
            ErrorHandling.clear();
            string code = "a = func(3, 2, true, false, \"abc\")";
            var statements = ParseCode(code);

            Assert.AreEqual(statements.Count, 1);
            Assert.IsTrue(statements[0] is AssignStatement);
            var statement = (AssignStatement)statements[0];
            Assert.IsTrue(statement.expression is FunctionCallExp);
            var exp = (FunctionCallExp)statement.expression;
            Assert.AreEqual(exp.attr.variableName, "func");
            Assert.AreEqual(exp.arguments.Count, 5);
            Assert.AreEqual(ErrorHandling.exceptions.Count, 0);
        }
    }
}
